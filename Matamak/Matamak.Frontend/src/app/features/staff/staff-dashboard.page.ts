import { Component, OnDestroy, computed, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { CatalogService } from '../../core/services/catalog.service';
import { OrderService } from '../../core/services/order.service';
import { StaffRealtimeService } from '../../core/services/staff-realtime.service';
import { Category, Country, MenuItem } from '../../core/models/catalog.models';

type StaffRole = 'Admin' | 'Cashier';
type DashboardTab = 'dashboard' | 'menu' | 'users' | 'orders' | 'inventory' | 'reports' | 'pos';

interface DashboardStat {
  label: string;
  value: string;
  note: string;
}

interface DashboardAction {
  label: string;
  description: string;
  route: string;
}

function getLocalTodayString(): string {
  const d = new Date();
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

@Component({
  selector: 'app-staff-dashboard',
  imports: [RouterLink, FormsModule, CommonModule],
  templateUrl: './staff-dashboard.page.html',
  styleUrl: './staff-dashboard.page.scss'
})
export class StaffDashboardPage implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly catalogService = inject(CatalogService);
  private readonly orderService = inject(OrderService);
  private readonly staffRealtime = inject(StaffRealtimeService);
  private readonly subscriptions = new Subscription();

  readonly user = this.authService.currentUser;
  readonly role = computed<StaffRole>(() => this.route.snapshot.data['role'] as StaffRole);
  readonly isAdmin = computed(() => this.role() === 'Admin');
  readonly isCashier = computed(() => this.role() === 'Cashier');
  readonly brandLink = computed(() => {
    if (this.isAdmin()) return '/admin';
    if (this.isCashier()) return '/cashier';
    return '/customer/menu';
  });
  readonly title = computed(() => (this.isAdmin() ? 'لوحة الإدارة' : 'واجهة الكاشير'));
  
  readonly subtitle = computed(() =>
    this.isAdmin()
      ? 'تابع أداء المطعم، المستخدمين، المنيو، والمخزون من مكان واحد.'
      : 'استقبل الطلبات، تابع حالتها، وأنه عمليات الدفع بسرعة.'
  );

  readonly stats = computed<DashboardStat[]>(() =>
    this.isAdmin()
      ? [
          { label: 'المستخدمون', value: 'إدارة', note: 'Admins, Cashiers, Customers' },
          { label: 'المنيو', value: 'أصناف', note: 'أطباق وتصنيفات ومطابخ' },
          { label: 'التقارير', value: 'مبيعات', note: 'ملخصات يومية وشهرية' }
        ]
      : [
          { label: 'طلبات الصالة', value: 'متاح', note: 'Dine-in orders' },
          { label: 'طلبات التوصيل', value: 'متاح', note: 'Delivery orders' },
          { label: 'Takeaway', value: 'متاح', note: 'طلبات الاستلام' },
          { label: 'المدفوعات', value: 'متابعة', note: 'Cashier payments' }
        ]
  );

  readonly actions = computed<DashboardAction[]>(() =>
    this.isAdmin()
      ? [
          { label: 'إدارة المنيو', description: 'أضف وعدل الأطباق والتصنيفات.', route: '/customer/menu' },
          { label: 'متابعة العملاء', description: 'راجع الحسابات والطلبات من لوحة التحكم.', route: '/admin' },
          { label: 'تقارير المبيعات', description: 'راقب الأداء واتجاهات الطلب.', route: '/admin' }
        ]
      : [
          { label: 'إنشاء طلب جديد', description: 'أنشئ طلبات سريعة من الأصناف المتاحة.', route: '' },
          { label: 'متابعة الطلبات', description: 'انتقل بين حالات التحضير والتسليم.', route: '' },
          { label: 'تسجيل الدفع', description: 'راجع المدفوعات المرتبطة بالطلبات.', route: '' }
        ]
  );
  
  // Navigation & Tab Signal
  readonly currentTab = signal<DashboardTab>('dashboard');

  // POS (Point of Sale) State
  readonly posCart = signal<{ item: MenuItem; quantity: number; note: string }[]>([]);
  posCustomerName = '';
  readonly isPosSubmitting = signal(false);
  readonly selectedPosCategory = signal<number | null>(null);
  readonly posSearchQuery = signal<string>('');
  readonly posOrderType = signal<'takeaway' | 'dinein'>('takeaway');
  readonly posTableNumber = signal<number | null>(null);

  readonly filteredPosItems = computed(() => {
    const catId = this.selectedPosCategory();
    const query = this.posSearchQuery().trim().toLowerCase();
    let allItems = this.items();
    if (catId !== null) {
      allItems = allItems.filter(item => item.catogryId === catId);
    }
    if (query) {
      allItems = allItems.filter(item => 
        (item.name || '').toLowerCase().includes(query) || 
        (item.description || '').toLowerCase().includes(query)
      );
    }
    return allItems;
  });

  readonly posTotal = computed(() => {
    return this.posCart().reduce((sum, item) => sum + (item.item.price * item.quantity), 0);
  });

  // Loaded Data Signals
  readonly items = signal<MenuItem[]>([]);
  readonly categories = signal<Category[]>([]);
  readonly countries = signal<Country[]>([]);
  readonly admins = signal<any[]>([]);
  readonly cashiers = signal<any[]>([]);
  readonly customers = signal<any[]>([]);
  readonly deliveryOrders = signal<any[]>([]);
  readonly takeawayOrders = signal<any[]>([]);
  readonly dineinOrders = signal<any[]>([]);
  readonly inventoryItems = signal<any[]>([]);
  readonly salesReport = signal<any>(null);

  // Orders date filter – defaults to today
  readonly ordersDateFilter = signal<string>(getLocalTodayString());

  private isSameDay(dateStr: any, filterDate: string): boolean {
    if (!dateStr) {
      return filterDate === getLocalTodayString();
    }
    try {
      const dateObj = new Date(dateStr);
      if (isNaN(dateObj.getTime())) {
        return String(dateStr).substring(0, 10) === filterDate;
      }
      const y = dateObj.getFullYear();
      const m = String(dateObj.getMonth() + 1).padStart(2, '0');
      const d = String(dateObj.getDate()).padStart(2, '0');
      return `${y}-${m}-${d}` === filterDate;
    } catch {
      return String(dateStr).substring(0, 10) === filterDate;
    }
  }

  readonly filteredDeliveryOrders = computed(() =>
    this.deliveryOrders().filter(o =>
      this.isSameDay(o.orderDate ?? o.OrderDate ?? o.createdAt ?? o.CreatedAt, this.ordersDateFilter())
    )
  );

  readonly filteredTakeawayOrders = computed(() =>
    this.takeawayOrders().filter(o =>
      this.isSameDay(o.orderDate ?? o.OrderDate ?? o.createdAt ?? o.CreatedAt, this.ordersDateFilter())
    )
  );

  readonly filteredDineinOrders = computed(() =>
    this.dineinOrders().filter(o =>
      this.isSameDay(o.orderDate ?? o.OrderDate ?? o.createdAt ?? o.CreatedAt, this.ordersDateFilter())
    )
  );

  todayStr(): string {
    return getLocalTodayString();
  }

  // Loading States
  readonly isLoading = signal(false);
  readonly operationMessage = signal<string | null>(null);
  readonly operationError = signal<string | null>(null);

  // Forms / Modals Data Model
  // 1. Menu Forms
  showItemModal = false;
  isUploadingImage = false;
  editingItem: MenuItem | null = null;
  itemForm = {
    name: '',
    description: '',
    price: 0,
    imageUrl: '',
    catogryId: 0,
    countryId: 0,
    isAvailable: true
  };
  newCategoryName = '';
  newCountryName = '';

  // 2. User Forms
  showUserModal = false;
  userForm = {
    username: '',
    fullName: '',
    email: '',
    phone: '',
    address: '',
    password: '',
    confirmPassword: ''
  };
  selectedUserRole = 'Cashier';

  // 3. Inventory Forms
  showInventoryModal = false;
  selectedInventoryItem: any = null;
  inventoryForm = {
    quantityInStock: 0,
    lowStockThreshold: 10
  };

  // 4. Report Filters
  reportFilter = {
    from: '',
    to: ''
  };

  // 5. Edit Order Forms
  showEditOrderModal = false;
  editingOrderType: 'takeaway' | 'dinein' | null = null;
  editingOrderId: number | null = null;
  editOrderForm = {
    customerName: '',
    tableNumber: 1,
    totalPrice: 0,
    items: [] as { name: string; priceForOne: number; quantity: number; note?: string; totalPrice: number }[]
  };

  ngOnInit(): void {
    this.loadCommonData();
    this.connectToRealtimeOrders();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    void this.staffRealtime.stop();
  }

  private connectToRealtimeOrders(): void {
    if (this.isAdmin()) {
      return;
    }

    this.subscriptions.add(
      this.staffRealtime.newDeliveryOrder$.subscribe((order) => {
        this.upsertDeliveryOrder(order);
        this.operationMessage.set('طلب توصيل جديد وصل للكاشير.');

        if (this.currentTab() === 'orders') {
          this.loadTabSpecificData('orders');
        }
      })
    );

    this.subscriptions.add(
      this.staffRealtime.deliveryOrderStatusChanged$.subscribe(({ order, message }) => {
        this.upsertDeliveryOrder(order);
        this.operationMessage.set(message || 'تم تحديث حالة طلب توصيل.');
      })
    );
    this.subscriptions.add(
      this.staffRealtime.newTakeawayOrder$.subscribe((order) => {
        this.upsertTakeawayOrder(order);
        this.operationMessage.set('طلب تيك أواي جديد وصل للكاشير.');
      })
    );

    this.subscriptions.add(
      this.staffRealtime.takeawayOrderRemoved$.subscribe((order) => {
        const id = order?.id ?? order?.Id;
        this.takeawayOrders.update((orders) => orders.filter((existing) => existing.id !== id && existing.Id !== id));
        this.operationMessage.set('تم إلغاء طلب تيك أواي.');
      })
    );

    this.subscriptions.add(
      this.staffRealtime.takeawayOrderStatusChanged$.subscribe(({ order, message }) => {
        this.upsertTakeawayOrder(order);
        this.operationMessage.set(message || 'تم تحديث حالة طلب تيك أواي.');
      })
    );

    void this.staffRealtime.start();
  }

  private upsertDeliveryOrder(order: any): void {
    const normalizedOrder = this.normalizeDeliveryOrder(order);
    const id = normalizedOrder.id ?? normalizedOrder.Id;

    this.deliveryOrders.update((orders) => {
      const existingIndex = orders.findIndex((existing) => existing.id === id || existing.Id === id);
      if (existingIndex === -1) {
        return [normalizedOrder, ...orders];
      }

      return orders.map((existing, index) => index === existingIndex ? { ...existing, ...normalizedOrder } : existing);
    });
  }

  private normalizeDeliveryOrder(order: any): any {
    return {
      ...order,
      id: order?.id ?? order?.Id,
      status: order?.status ?? order?.Status,
      orderNumber: order?.orderNumber ?? order?.OrderNumber,
      orderDate: order?.orderDate ?? order?.OrderDate,
      totalPrice: order?.totalPrice ?? order?.TotalPrice,
      deliveryAddress: order?.deliveryAddress ?? order?.DeliveryAddress,
      contactNumber: order?.contactNumber ?? order?.ContactNumber,
      customerName: order?.customerName ?? order?.CustomerName
    };
  }

  private upsertTakeawayOrder(order: any): void {
    const normalizedOrder = this.normalizeTakeawayOrder(order);
    const id = normalizedOrder.id ?? normalizedOrder.Id;

    this.takeawayOrders.update((orders) => {
      const existingIndex = orders.findIndex((existing) => existing.id === id || existing.Id === id);
      if (existingIndex === -1) {
        return [normalizedOrder, ...orders];
      }

      return orders.map((existing, index) => index === existingIndex ? { ...existing, ...normalizedOrder } : existing);
    });
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

  setTab(tab: DashboardTab): void {
    this.currentTab.set(tab);
    this.operationMessage.set(null);
    this.operationError.set(null);
    this.loadTabSpecificData(tab);
  }

  onActionClick(action: DashboardAction): void {
    if (action.label === 'إدارة المنيو') {
      this.setTab('menu');
    } else if (action.label === 'متابعة العملاء') {
      this.setTab('users');
    } else if (action.label === 'تقارير المبيعات') {
      this.setTab('reports');
    } else if (action.label === 'متابعة الطلبات' || action.label === 'تسجيل الدفع') {
      this.setTab('orders');
    } else if (action.label === 'فتح المنيو' || action.label === 'إنشاء طلب جديد') {
      this.setTab('pos');
    } else if (action.route) {
      void this.router.navigate([action.route]);
    }
  }

  loadCommonData(): void {
    this.catalogService.getCategories().subscribe(cats => this.categories.set(cats));
    this.catalogService.getCountries().subscribe(cnts => this.countries.set(cnts));
  }

  loadTabSpecificData(tab: DashboardTab): void {
    this.isLoading.set(true);
    switch (tab) {
      case 'dashboard':
        this.loadCommonData();
        break;
      case 'pos':
        this.loadCommonData();
        this.catalogService.getItems().subscribe({
          next: (res) => {
            this.items.set(res);
            this.isLoading.set(false);
          },
          error: () => this.isLoading.set(false)
        });
        break;
      case 'menu':
        this.catalogService.getItems().subscribe({
          next: (res) => {
            this.items.set(res);
            this.isLoading.set(false);
          },
          error: () => this.isLoading.set(false)
        });
        break;
      case 'users':
        if (this.isAdmin()) {
          this.authService.getAllAdmins().subscribe(res => this.admins.set(res));
          this.authService.getAllCashiers().subscribe(res => this.cashiers.set(res));
          this.authService.getAllCustomers().subscribe(res => {
            this.customers.set(res);
            this.isLoading.set(false);
          });
        } else {
          this.isLoading.set(false);
        }
        break;
      case 'orders':
        this.catalogService.getItems().subscribe(res => this.items.set(res));
        this.orderService.getDeliveryOrders().subscribe(res => this.deliveryOrders.set(res));
        this.orderService.getTakeawayOrders().subscribe(res => this.takeawayOrders.set(res));
        this.orderService.getDineinOrders().subscribe(res => {
          this.dineinOrders.set(res);
          this.isLoading.set(false);
        });
        break;
      case 'inventory':
        this.orderService.getInventoryItems().subscribe({
          next: (res) => {
            this.inventoryItems.set(res);
            this.isLoading.set(false);
          },
          error: () => this.isLoading.set(false)
        });
        break;
      case 'reports':
        if (this.isAdmin()) {
          this.fetchSalesReport();
        } else {
          this.isLoading.set(false);
        }
        break;
      default:
        this.isLoading.set(false);
    }
  }

  // ==========================================
  // MENU OPERATIONS (CRUD)
  // ==========================================
  openAddItemModal(): void {
    this.editingItem = null;
    this.itemForm = {
      name: '',
      description: '',
      price: 0,
      imageUrl: '',
      catogryId: this.categories()[0]?.id || 0,
      countryId: this.countries()[0]?.id || 0,
      isAvailable: true
    };
    this.showItemModal = true;
  }

  openEditItemModal(item: MenuItem): void {
    this.editingItem = item;
    this.itemForm = {
      name: item.name || '',
      description: item.description || '',
      price: item.price,
      imageUrl: item.imageUrl || '',
      catogryId: item.catogryId,
      countryId: item.countryId,
      isAvailable: item.isAvailable
    };
    this.showItemModal = true;
  }

  onFileSelected(event: Event): void {
    const element = event.currentTarget as HTMLInputElement;
    let fileList: FileList | null = element.files;
    if (fileList && fileList.length > 0) {
      const file = fileList[0];
      this.isUploadingImage = true;
      this.operationError.set(null);

      this.catalogService.uploadItemImage(file).subscribe({
        next: (res) => {
          this.itemForm.imageUrl = res.url;
          this.isUploadingImage = false;
        },
        error: (err) => {
          this.operationError.set('فشل رفع الصورة، يرجى المحاولة مرة أخرى.');
          this.isUploadingImage = false;
        }
      });
    }
  }

  saveItem(): void {
    this.operationMessage.set(null);
    this.operationError.set(null);

    const apiCall = this.editingItem
      ? this.catalogService.updateItem(this.editingItem.id, this.itemForm)
      : this.catalogService.addItem(this.itemForm);

    apiCall.subscribe({
      next: (msg) => {
        this.operationMessage.set(msg || 'تمت العملية بنجاح');
        this.showItemModal = false;
        this.loadTabSpecificData('menu');
      },
      error: (err) => this.operationError.set(err?.error || 'حدث خطأ أثناء حفظ المنتج')
    });
  }

  deleteItem(id: number): void {
    if (!confirm('هل أنت متأكد من حذف هذا المنتج؟')) return;
    this.catalogService.removeItem(id).subscribe({
      next: (msg) => {
        this.operationMessage.set(msg || 'تم الحذف بنجاح');
        this.loadTabSpecificData('menu');
      },
      error: (err) => this.operationError.set(err?.error || 'حدث خطأ أثناء الحذف')
    });
  }

  // Categories & Countries
  addCategory(): void {
    if (!this.newCategoryName.trim()) return;
    this.catalogService.addCategory(this.newCategoryName).subscribe({
      next: () => {
        this.newCategoryName = '';
        this.loadCommonData();
        this.operationMessage.set('تم إضافة القسم بنجاح');
      },
      error: (err) => this.operationError.set(err?.error || 'خطأ أثناء إضافة القسم')
    });
  }

  deleteCategory(id: number): void {
    if (!confirm('هل تريد حذف هذا القسم؟ قد يؤدي هذا لحذف الوجبات المرتبطة به.')) return;
    this.catalogService.removeCategory(id).subscribe({
      next: () => {
        this.loadCommonData();
        this.operationMessage.set('تم حذف القسم بنجاح');
      },
      error: (err) => this.operationError.set(err?.error || 'خطأ أثناء حذف القسم')
    });
  }

  addCountry(): void {
    if (!this.newCountryName.trim()) return;
    this.catalogService.addCountry(this.newCountryName).subscribe({
      next: () => {
        this.newCountryName = '';
        this.loadCommonData();
        this.operationMessage.set('تم إضافة المطبخ بنجاح');
      },
      error: (err) => this.operationError.set(err?.error || 'خطأ أثناء إضافة المطبخ')
    });
  }

  deleteCountry(id: number): void {
    if (!confirm('هل تريد حذف هذا المطبخ؟')) return;
    this.catalogService.removeCountry(id).subscribe({
      next: () => {
        this.loadCommonData();
        this.operationMessage.set('تم حذف المطبخ بنجاح');
      },
      error: (err) => this.operationError.set(err?.error || 'خطأ أثناء حذف المطبخ')
    });
  }

  // ==========================================
  // USER OPERATIONS
  // ==========================================
  openAddUserModal(): void {
    this.userForm = {
      username: '',
      fullName: '',
      email: '',
      phone: '',
      address: '',
      password: '',
      confirmPassword: ''
    };
    this.selectedUserRole = 'Cashier';
    this.showUserModal = true;
  }

  saveUser(): void {
    this.operationMessage.set(null);
    this.operationError.set(null);

    this.authService.registerStaff(this.userForm, this.selectedUserRole).subscribe({
      next: (msg) => {
        this.operationMessage.set(msg || 'تم إنشاء الحساب بنجاح');
        this.showUserModal = false;
        this.loadTabSpecificData('users');
      },
      error: (err) => this.operationError.set(err?.error || 'فشل إنشاء حساب الموظف')
    });
  }

  deleteUser(username: string): void {
    if (!confirm(`هل أنت متأكد من حذف حساب المستخدم: ${username}؟`)) return;
    this.authService.deleteAnyAccount(username).subscribe({
      next: (msg) => {
        this.operationMessage.set(msg || 'تم حذف الحساب بنجاح');
        this.loadTabSpecificData('users');
      },
      error: (err) => this.operationError.set(err?.error || 'فشل حذف الحساب')
    });
  }

  // ==========================================
  // ORDER CONTROL OPERATIONS
  // ==========================================
  updateDeliveryState(id: number, action: 'driver' | 'customer' | 'cancel'): void {
    const apiCall = action === 'driver'
      ? this.orderService.handOrderToDriver(id)
      : action === 'customer'
      ? this.orderService.handOrderToCustomer(id)
      : this.orderService.cancelDeliveryOrder(id);

    apiCall.subscribe({
      next: (msg) => {
        this.operationMessage.set(msg || 'تم تحديث حالة الطلب');
        this.loadTabSpecificData('orders');
      },
      error: (err) => this.operationError.set(err?.error || 'فشل تحديث حالة الطلب')
    });
  }

  updateDineinState(id: number, status: string): void {
    this.orderService.changeDineinStatus(id, status).subscribe({
      next: (msg) => {
        this.operationMessage.set(msg || 'تم تحديث حالة الطلب');
        this.loadTabSpecificData('orders');
      },
      error: (err) => this.operationError.set(err?.error || 'فشل تحديث حالة الطلب')
    });
  }

  cancelTakeaway(id: number): void {
    if (!confirm('هل تريد إلغاء هذا الطلب؟')) return;
    this.orderService.changeTakeawayOrderStatus(id, 'Canceled').subscribe({
      next: (msg) => {
        this.operationMessage.set(msg || 'تم إلغاء الطلب بنجاح');
        this.loadTabSpecificData('orders');
      },
      error: (err) => this.operationError.set(err?.error || 'فشل إلغاء الطلب')
    });
  }

  updateTakeawayState(id: number, status: string): void {
    this.orderService.changeTakeawayOrderStatus(id, status).subscribe({
      next: (msg) => {
        this.operationMessage.set(msg || 'تم تحديث حالة الطلب بنجاح');
        this.loadTabSpecificData('orders');
      },
      error: (err) => this.operationError.set(err?.error || 'فشل تحديث حالة الطلب')
    });
  }

  // ==========================================
  // EDIT ORDER OPERATIONS
  // ==========================================
  openEditOrderModal(order: any, type: 'takeaway' | 'dinein'): void {
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
      customerName: order.customerName ?? order.CustomerName ?? '',
      tableNumber: order.tableNumber ?? order.TableNumber ?? 1,
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
      this.operationError.set('يجب أن يحتوي الطلب على صنف واحد على الأقل.');
      return;
    }

    if (this.editingOrderType === 'takeaway') {
      if (!this.editOrderForm.customerName.trim()) {
        this.operationError.set('يرجى إدخال اسم العميل.');
        return;
      }
    } else if (this.editingOrderType === 'dinein') {
      if (!this.editOrderForm.tableNumber || this.editOrderForm.tableNumber <= 0) {
        this.operationError.set('يرجى تحديد رقم طاولة صحيح.');
        return;
      }
    }

    this.isLoading.set(true);
    this.operationMessage.set(null);
    this.operationError.set(null);

    const orderPayload = {
      customerName: this.editOrderForm.customerName,
      tableNumber: this.editOrderForm.tableNumber,
      totalPrice: this.editOrderForm.totalPrice,
      items: this.editOrderForm.items
    };

    const apiCall = this.editingOrderType === 'takeaway'
      ? this.orderService.updateTakeawayOrder(this.editingOrderId!, orderPayload)
      : this.orderService.updateDineinOrder(this.editingOrderId!, orderPayload);

    apiCall.subscribe({
      next: (msg) => {
        this.isLoading.set(false);
        this.operationMessage.set(msg || 'تم تعديل الطلب بنجاح');
        this.closeEditOrderModal();
        this.loadTabSpecificData('orders');
      },
      error: (err) => {
        this.isLoading.set(false);
        this.operationError.set(err?.error || 'حدث خطأ أثناء تعديل الطلب.');
      }
    });
  }


  // ==========================================
  // INVENTORY OPERATIONS
  // ==========================================
  openEditStockModal(item: any): void {
    this.selectedInventoryItem = item;
    this.inventoryForm = {
      quantityInStock: item.quantityInStock,
      lowStockThreshold: item.lowStockThreshold
    };
    this.showInventoryModal = true;
  }

  saveStock(): void {
    this.orderService.updateInventoryItem(
      this.selectedInventoryItem.itemId,
      this.inventoryForm.quantityInStock,
      this.inventoryForm.lowStockThreshold
    ).subscribe({
      next: () => {
        this.operationMessage.set('تم تحديث بيانات المخزون بنجاح');
        this.showInventoryModal = false;
        this.loadTabSpecificData('inventory');
      },
      error: (err) => this.operationError.set(err?.error || 'فشل تحديث بيانات المخزون')
    });
  }

  // ==========================================
  // REPORTS OPERATIONS
  // ==========================================
  fetchSalesReport(): void {
    this.isLoading.set(true);
    this.orderService.getSalesReport(this.reportFilter.from, this.reportFilter.to).subscribe({
      next: (res) => {
        this.salesReport.set(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.salesReport.set(null);
        this.isLoading.set(false);
      }
    });
  }

  // ==========================================
  // POS (POINT OF SALE) OPERATIONS
  // ==========================================
  selectPosCategory(id: number | null): void {
    this.selectedPosCategory.set(id);
  }

  addToPosCart(item: MenuItem): void {
    if (!item.isAvailable) return;
    const current = this.posCart();
    const existing = current.find(i => i.item.id === item.id);
    if (existing) {
      this.posCart.set(current.map(i => i.item.id === item.id ? { ...i, quantity: i.quantity + 1 } : i));
    } else {
      this.posCart.set([...current, { item, quantity: 1, note: '' }]);
    }
  }

  updatePosQuantity(itemId: number, delta: number): void {
    const current = this.posCart();
    const existing = current.find(i => i.item.id === itemId);
    if (!existing) return;
    const newQty = existing.quantity + delta;
    if (newQty <= 0) {
      this.posCart.set(current.filter(i => i.item.id !== itemId));
    } else {
      this.posCart.set(current.map(i => i.item.id === itemId ? { ...i, quantity: newQty } : i));
    }
  }

  updatePosNote(itemId: number, note: string): void {
    this.posCart.set(this.posCart().map(i => i.item.id === itemId ? { ...i, note } : i));
  }

  clearPosCart(): void {
    this.posCart.set([]);
    this.posCustomerName = '';
    this.posOrderType.set('takeaway');
    this.posTableNumber.set(null);
  }

  submitPosOrder(): void {
    if (this.posCart().length === 0) {
      this.operationError.set('سلة المشتريات فارغة.');
      return;
    }

    if (this.posOrderType() === 'takeaway') {
      if (!this.posCustomerName.trim()) {
        this.operationError.set('يرجى إدخال اسم العميل.');
        return;
      }
    } else {
      if (!this.posTableNumber() || this.posTableNumber()! <= 0) {
        this.operationError.set('يرجى تحديد رقم طاولة صحيح (أكبر من 0).');
        return;
      }
    }

    this.isPosSubmitting.set(true);
    this.operationMessage.set(null);
    this.operationError.set(null);

    const itemsDto = this.posCart().map(cartItem => ({
      name: cartItem.item.name || 'صنف',
      priceForOne: cartItem.item.price,
      quantity: cartItem.quantity,
      note: cartItem.note,
      totalPrice: cartItem.item.price * cartItem.quantity
    }));

    if (this.posOrderType() === 'takeaway') {
      const takeawayPayload = {
        customerName: this.posCustomerName,
        totalPrice: this.posTotal(),
        items: itemsDto
      };

      this.orderService.createTakeawayOrder(takeawayPayload).subscribe({
        next: (res) => {
          this.isPosSubmitting.set(false);
          this.clearPosCart();
          this.operationMessage.set('تم إنشاء طلب التيك أواي بنجاح!');
          this.setTab('orders');
        },
        error: (err) => {
          this.isPosSubmitting.set(false);
          this.operationError.set(err?.error || err?.message || 'فشل إنشاء الطلب.');
        }
      });
    } else {
      const dineinPayload = {
        tableNumber: this.posTableNumber()!,
        totalPrice: this.posTotal(),
        items: itemsDto
      };

      this.orderService.createDineinOrder(dineinPayload).subscribe({
        next: (res) => {
          this.isPosSubmitting.set(false);
          this.clearPosCart();
          this.operationMessage.set('تم إنشاء طلب الصالة بنجاح!');
          this.setTab('orders');
        },
        error: (err) => {
          this.isPosSubmitting.set(false);
          this.operationError.set(err?.error || err?.message || 'فشل إنشاء الطلب.');
        }
      });
    }
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => void this.router.navigate(['/auth/login']),
      error: () => {
        this.authService.clearSession();
        void this.router.navigate(['/auth/login']);
      }
    });
  }
}




