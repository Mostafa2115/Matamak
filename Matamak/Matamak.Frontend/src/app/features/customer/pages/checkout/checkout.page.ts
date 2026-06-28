import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../core/services/auth.service';
import { CartService } from '../../../../core/services/cart.service';
import { OrderService, OrderItemDto } from '../../../../core/services/order.service';
import { ApiError } from '../../../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-checkout-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, DecimalPipe],
  templateUrl: './checkout.page.html',
  styleUrl: './checkout.page.scss'
})
export class CheckoutPage {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly orderService = inject(OrderService);
  
  readonly cartService = inject(CartService);

  readonly isCashier = computed(() => this.authService.currentUser()?.roles.includes('Cashier') ?? false);
  readonly orderType = signal<'delivery' | 'takeaway'>('delivery');
  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly orderSuccess = signal(false);

  readonly deliveryFee = 30;

  readonly form = this.fb.group({
    customerName: [this.authService.currentUser()?.username || '', Validators.required],
    contactNumber: ['', [Validators.required, Validators.pattern(/^[0-9+ ]{8,15}$/)]],
    deliveryAddress: ['', [Validators.required, Validators.minLength(20)]]
  });

  constructor() {
    const cashier = this.authService.currentUser()?.roles.includes('Cashier') ?? false;
    if (cashier) {
      this.orderType.set('takeaway');
      this.form.controls.deliveryAddress.clearValidators();
      this.form.controls.contactNumber.clearValidators();
      this.form.controls.deliveryAddress.updateValueAndValidity();
      this.form.controls.contactNumber.updateValueAndValidity();
      this.form.controls.customerName.setValue('');
    }
  }

  readonly orderTotal = computed(() => {
    const subtotal = this.cartService.totalPrice();
    return this.orderType() === 'delivery' ? subtotal + this.deliveryFee : subtotal;
  });

  setOrderType(type: 'delivery' | 'takeaway'): void {
    this.orderType.set(type);
    
    // Dynamically adjust validation based on order type
    const addressControl = this.form.controls.deliveryAddress;
    const phoneControl = this.form.controls.contactNumber;

    if (type === 'delivery') {
      addressControl.setValidators([Validators.required, Validators.minLength(20)]);
      phoneControl.setValidators([Validators.required, Validators.pattern(/^[0-9+ ]{8,15}$/)]);
    } else {
      addressControl.clearValidators();
      phoneControl.clearValidators();
    }
    
    addressControl.updateValueAndValidity();
    phoneControl.updateValueAndValidity();
  }

  submitOrder(): void {
    if (this.cartService.items().length === 0) {
      this.errorMessage.set('سلة المشتريات فارغة.');
      return;
    }

    this.form.markAllAsTouched();
    if (this.form.invalid || this.isSubmitting()) {
      return;
    }

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    const itemsDto: OrderItemDto[] = this.cartService.items().map((cartItem) => ({
      name: cartItem.item.name || 'صنف',
      priceForOne: cartItem.item.price,
      quantity: cartItem.quantity,
      note: cartItem.note,
      totalPrice: cartItem.item.price * cartItem.quantity
    }));

    if (this.orderType() === 'delivery') {
      const deliveryPayload = {
        customerName: this.form.controls.customerName.value || '',
        contactNumber: this.form.controls.contactNumber.value || '',
        deliveryAddress: this.form.controls.deliveryAddress.value || '',
        totalPrice: this.orderTotal(),
        items: itemsDto
      };

      this.orderService.createDeliveryOrder(deliveryPayload)
        .pipe(finalize(() => this.isSubmitting.set(false)))
        .subscribe({
          next: () => this.handleOrderSuccess(),
          error: (err: ApiError) => this.errorMessage.set(err.message)
        });
    } else {
      const takeawayPayload = {
        customerName: this.form.controls.customerName.value || '',
        totalPrice: this.orderTotal(),
        items: itemsDto
      };

      this.orderService.createTakeawayOrder(takeawayPayload)
        .pipe(finalize(() => this.isSubmitting.set(false)))
        .subscribe({
          next: () => this.handleOrderSuccess(),
          error: (err: ApiError) => this.errorMessage.set(err.message)
        });
    }
  }

  private handleOrderSuccess(): void {
    this.orderSuccess.set(true);
    this.cartService.clearCart();
    const roles = this.authService.currentUser()?.roles || [];
    setTimeout(() => {
      if (roles.includes('Cashier')) {
        void this.router.navigate(['/cashier']);
      } else if (roles.includes('Admin')) {
        void this.router.navigate(['/admin']);
      } else {
        void this.router.navigate(['/customer/menu']);
      }
    }, 4000);
  }
}
