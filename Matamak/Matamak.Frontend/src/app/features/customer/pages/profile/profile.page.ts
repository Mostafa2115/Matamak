import { Component, OnDestroy, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { DatePipe, NgClass } from '@angular/common';
import { Subscription } from 'rxjs';
import { ApiError } from '../../../../core/interceptors/error.interceptor';
import { CustomerUser } from '../../../../core/models/user.models';
import { AuthService } from '../../../../core/services/auth.service';
import { OrderService } from '../../../../core/services/order.service';
import { StaffRealtimeService } from '../../../../core/services/staff-realtime.service';
import { EditAccountRequest, ChangePasswordRequest } from '../../../../core/models/auth.models';
import { DeliveryOrderSummary } from '../../../../core/models/order.models';
import { MenuItem } from '../../../../core/models/catalog.models';
import { CatalogService } from '../../../../core/services/catalog.service';

type ProfileTab = 'info' | 'password' | 'orders';

interface OrderStatusStep {
  key: string;
  label: string;
  active: boolean;
  done: boolean;
}

const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const newPassword = control.get('newPassword');
  const confirmPassword = control.get('confirmPassword');
  return newPassword && confirmPassword && newPassword.value !== confirmPassword.value
    ? { passwordMismatch: true }
    : null;
};

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [RouterLink, FormsModule, ReactiveFormsModule, DatePipe, NgClass],
  templateUrl: './profile.page.html',
  styleUrl: './profile.page.scss'
})
export class ProfilePage implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly orderService = inject(OrderService);
  private readonly realtimeService = inject(StaffRealtimeService);
  private readonly catalogService = inject(CatalogService);
  private readonly subscriptions = new Subscription();

  readonly user = this.authService.currentUser;
  readonly profile = signal<CustomerUser | null>(null);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly updatingOrderId = signal<number | null>(null);
  readonly orderActionMessage = signal<string | null>(null);
  readonly orderActionError = signal<string | null>(null);

  // Edit Order Modal States
  showEditOrderModal = false;
  editingOrderType: 'delivery' | 'takeaway' | null = null;
  editingOrderId: number | null = null;
  editOrderForm = {
    customerName: '',
    deliveryAddress: '',
    contactNumber: '',
    totalPrice: 0,
    items: [] as { name: string; priceForOne: number; quantity: number; note?: string; totalPrice: number }[]
  };

  readonly items = signal<MenuItem[]>([]);

  // Active tab state
  readonly activeTab = signal<ProfileTab>('info');

  // Editing profile state
  readonly isEditing = signal(false);
  readonly isSavingProfile = signal(false);
  readonly profileSuccessMessage = signal<string | null>(null);
  readonly profileErrorMessage = signal<string | null>(null);

  // Password change state
  readonly isChangingPassword = signal(false);
  readonly passwordSuccessMessage = signal<string | null>(null);
  readonly passwordErrorMessage = signal<string | null>(null);

  // Forms
  readonly profileForm = this.fb.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    phoneNumber: ['', [Validators.required, Validators.pattern(/^[0-9+ ]{8,15}$/)]],
    address: ['', [Validators.required, Validators.minLength(10)]]
  });

  readonly passwordForm = this.fb.group({
    oldPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: passwordMatchValidator });

  ngOnInit(): void {
    const username = this.user()?.username;
    if (!username) {
      this.isLoading.set(false);
      this.errorMessage.set('لا يمكن تحميل بيانات العميل قبل تسجيل الدخول.');
      return;
    }
    this.route.queryParamMap.subscribe((params) => {
      const tab = params.get('tab');
      if (this.isProfileTab(tab)) {
        this.activeTab.set(tab as ProfileTab);
      }
    });

    this.connectToOrderUpdates();
    this.loadProfile(username);

    this.catalogService.getItems().subscribe({
      next: (res) => this.items.set(res),
      error: () => {}
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    void this.realtimeService.stop();
  }

  private connectToOrderUpdates(): void {
    // Start connection first
    void this.realtimeService.start();

    this.subscriptions.add(
      this.realtimeService.deliveryOrderStatusChanged$.subscribe(({ order, message }) => {
        this.updateProfileOrder(order);
        this.orderActionMessage.set(message || 'تم تحديث حالة الطلب.');
      })
    );

    this.subscriptions.add(
      this.realtimeService.takeawayOrderStatusChanged$.subscribe(({ order, message }) => {
        // Reload from server to avoid casing mismatch issues with in-memory update
        this.orderActionMessage.set(message || 'تم تحديث حالة طلب التيك أواي.');
        this.silentReloadProfile();
      })
    );

    this.subscriptions.add(
      this.realtimeService.takeawayOrderRemoved$.subscribe((order) => {
        this.orderActionMessage.set('تم إلغاء أو إزالة طلب التيك أواي.');
        this.silentReloadProfile();
      })
    );
  }

  private updateProfileOrder(order: DeliveryOrderSummary): void {
    const normalizedOrder = this.normalizeDeliveryOrder(order);
    this.profile.update((profile) => {
      if (!profile) {
        return profile;
      }

      return {
        ...profile,
        deliveryOrders: profile.deliveryOrders.map((existingOrder) =>
          existingOrder.id === normalizedOrder.id ? { ...existingOrder, ...normalizedOrder } : existingOrder
        )
      };
    });
  }

  private updateProfileTakeawayOrder(order: any): void {
    const normalizedOrder = this.normalizeTakeawayOrder(order);
    const id = normalizedOrder.id ?? normalizedOrder.Id;
    this.profile.update((profile) => {
      if (!profile) {
        return profile;
      }

      return {
        ...profile,
        takeawayOrders: profile.takeawayOrders.map((existingOrder) =>
          existingOrder.id === id ? { ...existingOrder, ...normalizedOrder } : existingOrder
        )
      };
    });
  }

  private normalizeDeliveryOrder(order: any): DeliveryOrderSummary {
    return {
      id: order?.id ?? order?.Id,
      status: order?.status ?? order?.Status,
      orderNumber: order?.orderNumber ?? order?.OrderNumber,
      orderDate: order?.orderDate ?? order?.OrderDate,
      totalPrice: order?.totalPrice ?? order?.TotalPrice
    };
  }

  private normalizeTakeawayOrder(order: any): any {
    return {
      ...order,
      id: order?.id ?? order?.Id,
      status: order?.status ?? order?.Status,
      orderNumber: order?.orderNumber ?? order?.OrderNumber,
      orderDate: order?.orderDate ?? order?.OrderDate,
      totalPrice: order?.totalPrice ?? order?.TotalPrice,
      customerName: order?.customerName ?? order?.CustomerName,
      isPaid: order?.isPaid ?? order?.IsPaid
    };
  }

  loadProfile(username = this.user()?.username): void {
    if (!username) {
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authService.getCustomer(username).subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.isLoading.set(false);
        this.populateProfileForm(profile);
      },
      error: (error: ApiError) => {
        this.profile.set(null);
        this.errorMessage.set(error.message);
        this.isLoading.set(false);
      }
    });
  }

  private silentReloadProfile(): void {
    const username = this.user()?.username;
    if (!username) {
      return;
    }
    // Reload without showing the full-page loading spinner
    this.authService.getCustomer(username).subscribe({
      next: (profile) => {
        this.profile.set(profile);
      },
      error: () => { /* silently ignore refresh errors */ }
    });
  }


  setTab(tab: ProfileTab): void {
    this.activeTab.set(tab);
    // Reset messages when switching tabs
    this.profileSuccessMessage.set(null);
    this.profileErrorMessage.set(null);
    this.passwordSuccessMessage.set(null);
    this.passwordErrorMessage.set(null);
  }

  populateProfileForm(profile: CustomerUser): void {
    this.profileForm.patchValue({
      fullName: profile.fullName || '',
      phoneNumber: profile.phoneNumber || '',
      address: profile.address || ''
    });
  }

  toggleEdit(editing: boolean): void {
    this.isEditing.set(editing);
    if (!editing && this.profile()) {
      // Revert edits to actual loaded profile details
      this.populateProfileForm(this.profile()!);
    }
    this.profileSuccessMessage.set(null);
    this.profileErrorMessage.set(null);
  }


  getOrderStatusLabel(status?: string): string {
    switch (this.normalizeStatus(status)) {
      case 'pending':
      case 'paid':
      case 'processing':
      case 'preparing':
        return 'جاري التحضير';
      case 'ready':
        return '🔔 جاهز للاستلام!';
      case 'outfordelivery':
      case 'withdriver':
        return 'مع المندوب';
      case 'delivered':
      case 'completed':
        return 'تم التسليم';
      case 'canceled':
      case 'cancelled':
      case 'rejected':
        return 'تم الإلغاء';
      default:
        return status || 'قيد المراجعة';
    }
  }

  getOrderStatusHint(status?: string): string {
    switch (this.normalizeStatus(status)) {
      case 'pending':
      case 'paid':
      case 'processing':
      case 'preparing':
        return 'الفريق يجهز طلبك الآن.';
      case 'ready':
        return 'طلبك جاهز! توجه للمطعم لاستلامه الآن.';
      case 'outfordelivery':
      case 'withdriver':
        return 'طلبك خرج للتوصيل وفي الطريق إليك.';
      case 'delivered':
      case 'completed':
        return 'تم تسليم الطلب بنجاح.';
      case 'canceled':
      case 'cancelled':
      case 'rejected':
        return 'تم إلغاء الطلب.';
      default:
        return 'سنعرض آخر تحديث لحالة الطلب هنا.';
    }
  }

  getOrderStatusClass(status?: string): string {
    return `status-${this.normalizeStatus(status) || 'unknown'}`;
  }

  getOrderTimeline(order: DeliveryOrderSummary): OrderStatusStep[] {
    const status = this.normalizeStatus(order.status);
    const isCanceled = ['canceled', 'cancelled', 'rejected'].includes(status);
    const currentIndex = this.getStatusIndex(status);
    const steps = [
      { key: 'pending', label: 'تم الاستلام' },
      { key: 'preparing', label: 'قيد التحضير' },
      { key: 'outfordelivery', label: 'مع المندوب' },
      { key: 'delivered', label: 'تم التسليم' }
    ];

    if (isCanceled) {
      return [
        { key: 'pending', label: 'تم الاستلام', active: false, done: true },
        { key: 'canceled', label: 'تم الإلغاء', active: true, done: true }
      ];
    }

    return steps.map((step, index) => ({
      ...step,
      active: index === currentIndex,
      done: index <= currentIndex
    }));
  }

  private getStatusIndex(status: string): number {
    switch (status) {
      case 'paid':
      case 'processing':
      case 'preparing':
        return 1;
      case 'outfordelivery':
      case 'withdriver':
        return 2;
      case 'delivered':
      case 'completed':
        return 3;
      case 'pending':
      default:
        return 0;
    }
  }

  private normalizeStatus(status?: string): string {
    return (status || '').replace(/\s|_|-/g, '').toLowerCase();
  }

  private isProfileTab(tab: string | null): tab is ProfileTab {
    return tab === 'info' || tab === 'password' || tab === 'orders';
  }

  canCancelOrder(status?: string): boolean {
    const normalizedStatus = this.normalizeStatus(status);
    return !['outfordelivery', 'withdriver', 'delivered', 'completed', 'canceled', 'cancelled', 'rejected'].includes(normalizedStatus);
  }

  canMarkOrderDelivered(status?: string): boolean {
    return ['outfordelivery', 'withdriver'].includes(this.normalizeStatus(status));
  }

  cancelOrder(order: DeliveryOrderSummary): void {
    this.updateDeliveryOrder(order, 'cancel');
  }

  cancelTakeawayOrder(order: any): void {
    const id = order.id ?? order.Id;
    const username = this.user()?.username;
    if (!username || this.updatingOrderId()) {
      return;
    }

    if (!confirm('هل أنت متأكد من إلغاء طلب التيك أواي؟')) {
      return;
    }

    this.updatingOrderId.set(id);
    this.orderActionMessage.set(null);
    this.orderActionError.set(null);

    this.orderService.changeTakeawayOrderStatus(id, 'Canceled').subscribe({
      next: () => {
        this.orderActionMessage.set('تم إلغاء الطلب بنجاح.');
        this.updatingOrderId.set(null);
        this.loadProfile(username);
      },
      error: (error: ApiError) => {
        this.orderActionError.set(error.message || 'تعذر إلغاء الطلب حالياً.');
        this.updatingOrderId.set(null);
      }
    });
  }

  markOrderDelivered(order: DeliveryOrderSummary): void {
    this.updateDeliveryOrder(order, 'deliver');
  }

  private updateDeliveryOrder(order: DeliveryOrderSummary, action: 'cancel' | 'deliver'): void {
    const username = this.user()?.username;
    if (!username || this.updatingOrderId()) {
      return;
    }

    this.updatingOrderId.set(order.id);
    this.orderActionMessage.set(null);
    this.orderActionError.set(null);

    const request = action === 'cancel'
      ? this.orderService.cancelDeliveryOrder(order.id)
      : this.orderService.handOrderToCustomer(order.id);

    request.subscribe({
      next: () => {
        this.orderActionMessage.set(action === 'cancel' ? 'تم إلغاء الطلب بنجاح.' : 'تم تأكيد استلام الطلب.');
        this.updatingOrderId.set(null);
        this.loadProfile(username);
      },
      error: (error: ApiError) => {
        this.orderActionError.set(error.message || 'تعذر تحديث حالة الطلب حالياً.');
        this.updatingOrderId.set(null);
      }
    });
  }

  onEditProfileSubmit(): void {
    this.profileForm.markAllAsTouched();
    if (this.profileForm.invalid || this.isSavingProfile()) {
      return;
    }

    const username = this.user()?.username;
    if (!username) {
      this.profileErrorMessage.set('انتهت صلاحية الجلسة، يرجى تسجيل الدخول مرة أخرى.');
      return;
    }

    this.isSavingProfile.set(true);
    this.profileSuccessMessage.set(null);
    this.profileErrorMessage.set(null);

    // Note the exact casing matching EditAccountRequest
    const payload: EditAccountRequest = {
      FullName: this.profileForm.value.fullName || '',
      Address: this.profileForm.value.address || '',
      username: username,
      phonenumber: this.profileForm.value.phoneNumber || ''
    };

    this.authService.editAccount(username, payload).subscribe({
      next: () => {
        this.profileSuccessMessage.set('تم تحديث البيانات الشخصية بنجاح.');
        this.isSavingProfile.set(false);
        this.isEditing.set(false);
        // Reload details to sync everything
        this.loadProfile(username);
      },
      error: (error: ApiError) => {
        this.profileErrorMessage.set(error.message || 'حدث خطأ أثناء حفظ التعديلات.');
        this.isSavingProfile.set(false);
      }
    });
  }

  onChangePasswordSubmit(): void {
    this.passwordForm.markAllAsTouched();
    if (this.passwordForm.invalid || this.isChangingPassword()) {
      return;
    }

    const username = this.user()?.username;
    if (!username) {
      this.passwordErrorMessage.set('انتهت صلاحية الجلسة، يرجى تسجيل الدخول مرة أخرى.');
      return;
    }

    this.isChangingPassword.set(true);
    this.passwordSuccessMessage.set(null);
    this.passwordErrorMessage.set(null);

    const payload: ChangePasswordRequest = {
      oldPassword: this.passwordForm.value.oldPassword || '',
      newPassword: this.passwordForm.value.newPassword || '',
      confirmPassword: this.passwordForm.value.confirmPassword || ''
    };

    this.authService.changePassword(username, payload).subscribe({
      next: () => {
        this.passwordSuccessMessage.set('تم تغيير كلمة المرور بنجاح.');
        this.isChangingPassword.set(false);
        this.passwordForm.reset();
      },
      error: (error: ApiError) => {
        this.passwordErrorMessage.set(error.message || 'حدث خطأ أثناء تغيير كلمة المرور.');
        this.isChangingPassword.set(false);
      }
    });
  }

  canEditOrder(status?: string, type?: 'delivery' | 'takeaway'): boolean {
    const normalizedStatus = this.normalizeStatus(status);
    if (type === 'delivery') {
      return ['pending', 'paid'].includes(normalizedStatus);
    } else {
      return ['pending', 'paid', 'preparing'].includes(normalizedStatus);
    }
  }

  openEditOrderModal(order: any, type: 'delivery' | 'takeaway'): void {
    this.editingOrderType = type;
    this.editingOrderId = order.id ?? order.Id;
    
    // Deep copy order items
    const rawItems = order.items ?? order.Items ?? [];
    const copiedItems = rawItems.map((item: any) => ({
      name: item.name ?? item.Name,
      priceForOne: item.priceForOne ?? item.PriceForOne,
      quantity: item.quantity ?? item.Quantity,
      note: item.note ?? item.Note,
      totalPrice: item.totalPrice ?? item.TotalPrice
    }));

    this.editOrderForm = {
      customerName: order.customerName ?? order.CustomerName ?? this.user()?.username ?? '',
      deliveryAddress: order.deliveryAddress ?? order.DeliveryAddress ?? '',
      contactNumber: order.contactNumber ?? order.ContactNumber ?? '',
      totalPrice: order.totalPrice ?? order.TotalPrice ?? 0,
      items: copiedItems
    };
    this.showEditOrderModal = true;
  }

  closeEditOrderModal(): void {
    this.showEditOrderModal = false;
    this.editingOrderType = null;
    this.editingOrderId = null;
  }

  addPlaylistItemToEditOrder(catalogItem: MenuItem): void {
    const itemName = catalogItem.name || 'صنف';
    const existing = this.editOrderForm.items.find(i => i.name === itemName);
    if (existing) {
      existing.quantity += 1;
      existing.totalPrice = existing.priceForOne * existing.quantity;
    } else {
      this.editOrderForm.items.push({
        name: itemName,
        priceForOne: catalogItem.price,
        quantity: 1,
        note: '',
        totalPrice: catalogItem.price
      });
    }
    this.recalculateEditOrderTotal();
  }

  updateEditOrderItemQty(index: number, delta: number): void {
    const item = this.editOrderForm.items[index];
    if (!item) return;
    item.quantity += delta;
    if (item.quantity <= 0) {
      this.editOrderForm.items.splice(index, 1);
    } else {
      item.totalPrice = item.priceForOne * item.quantity;
    }
    this.recalculateEditOrderTotal();
  }

  updateEditOrderItemNote(index: number, note: string): void {
    const item = this.editOrderForm.items[index];
    if (item) {
      item.note = note;
    }
  }

  recalculateEditOrderTotal(): void {
    this.editOrderForm.totalPrice = this.editOrderForm.items.reduce((sum, item) => sum + item.totalPrice, 0);
  }

  saveEditOrder(): void {
    if (this.editOrderForm.items.length === 0) {
      this.orderActionError.set('يجب أن يحتوي الطلب على صنف واحد على الأقل.');
      return;
    }

    if (this.editingOrderType === 'delivery') {
      if (!this.editOrderForm.deliveryAddress.trim() || this.editOrderForm.deliveryAddress.length < 10) {
        this.orderActionError.set('يرجى إدخال عنوان توصيل صحيح بالتفصيل (10 أحرف على الأقل).');
        return;
      }
      if (!this.editOrderForm.contactNumber.trim() || !/^[0-9+ ]{8,15}$/.test(this.editOrderForm.contactNumber)) {
        this.orderActionError.set('يرجى إدخال رقم هاتف تواصل صحيح.');
        return;
      }
    }

    this.isLoading.set(true);
    this.orderActionMessage.set(null);
    this.orderActionError.set(null);

    const orderPayload = {
      customerName: this.editOrderForm.customerName,
      deliveryAddress: this.editOrderForm.deliveryAddress,
      contactNumber: this.editOrderForm.contactNumber,
      totalPrice: this.editOrderForm.totalPrice,
      items: this.editOrderForm.items
    };

    const apiCall = this.editingOrderType === 'takeaway'
      ? this.orderService.updateTakeawayOrder(this.editingOrderId!, {
          customerName: this.editOrderForm.customerName,
          totalPrice: this.editOrderForm.totalPrice,
          items: this.editOrderForm.items
        })
      : this.orderService.updateDeliveryOrder(this.editingOrderId!, orderPayload);

    apiCall.subscribe({
      next: (msg) => {
        this.isLoading.set(false);
        this.orderActionMessage.set(msg || 'تم تعديل الطلب بنجاح');
        this.closeEditOrderModal();
        this.loadProfile();
      },
      error: (err) => {
        this.isLoading.set(false);
        this.orderActionError.set(err?.error || 'حدث خطأ أثناء تعديل الطلب.');
      }
    });
  }
}







