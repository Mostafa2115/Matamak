import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { CartService } from '../../../../core/services/cart.service';

@Component({
  selector: 'app-cart-drawer',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './cart-drawer.component.html',
  styleUrl: './cart-drawer.component.scss'
})
export class CartDrawerComponent {
  private readonly router = inject(Router);
  readonly cartService = inject(CartService);

  @Input() isOpen = false;
  @Output() closeDrawer = new EventEmitter<void>();

  close(): void {
    this.closeDrawer.emit();
  }

  incrementQuantity(itemId: number, currentQuantity: number): void {
    this.cartService.updateQuantity(itemId, currentQuantity + 1);
  }

  decrementQuantity(itemId: number, currentQuantity: number): void {
    this.cartService.updateQuantity(itemId, currentQuantity - 1);
  }

  removeItem(itemId: number): void {
    this.cartService.removeFromCart(itemId);
  }

  goToCheckout(): void {
    this.close();
    void this.router.navigate(['/customer/checkout']);
  }
}
