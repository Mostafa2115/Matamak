import { computed, Injectable, signal } from '@angular/core';
import { MenuItem } from '../models/catalog.models';
import { CartItem } from '../models/order.models';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly STORAGE_KEY = 'matamak_cart';
  private readonly itemsSignal = signal<CartItem[]>(this.loadCartFromStorage());

  readonly isDrawerOpen = signal(false);
  readonly items = this.itemsSignal.asReadonly();
  readonly itemCount = computed(() => this.itemsSignal().reduce((acc, curr) => acc + curr.quantity, 0));
  readonly totalPrice = computed(() => this.itemsSignal().reduce((acc, curr) => acc + (curr.quantity * curr.item.price), 0));

  openDrawer(): void {
    this.isDrawerOpen.set(true);
  }

  closeDrawer(): void {
    this.isDrawerOpen.set(false);
  }

  addToCart(item: MenuItem, quantity = 1, note?: string): void {
    const currentItems = this.itemsSignal();
    const existingIndex = currentItems.findIndex((i) => i.item.id === item.id);

    let updatedItems: CartItem[];
    if (existingIndex > -1) {
      updatedItems = currentItems.map((cartItem, index) =>
        index === existingIndex
          ? { ...cartItem, quantity: cartItem.quantity + quantity, note: note || cartItem.note }
          : cartItem
      );
    } else {
      updatedItems = [...currentItems, { item, quantity, note }];
    }

    this.itemsSignal.set(updatedItems);
    this.saveCartToStorage(updatedItems);
  }

  removeFromCart(itemId: number): void {
    const updatedItems = this.itemsSignal().filter((i) => i.item.id !== itemId);
    this.itemsSignal.set(updatedItems);
    this.saveCartToStorage(updatedItems);
  }

  updateQuantity(itemId: number, quantity: number): void {
    if (quantity <= 0) {
      this.removeFromCart(itemId);
      return;
    }

    const updatedItems = this.itemsSignal().map((i) =>
      i.item.id === itemId ? { ...i, quantity } : i
    );
    this.itemsSignal.set(updatedItems);
    this.saveCartToStorage(updatedItems);
  }

  clearCart(): void {
    this.itemsSignal.set([]);
    localStorage.removeItem(this.STORAGE_KEY);
  }

  private saveCartToStorage(items: CartItem[]): void {
    try {
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(items));
    } catch (e) {
      console.error('Error saving cart to storage', e);
    }
  }

  private loadCartFromStorage(): CartItem[] {
    try {
      const stored = localStorage.getItem(this.STORAGE_KEY);
      return stored ? (JSON.parse(stored) as CartItem[]) : [];
    } catch (e) {
      console.error('Error loading cart from storage', e);
      return [];
    }
  }
}
