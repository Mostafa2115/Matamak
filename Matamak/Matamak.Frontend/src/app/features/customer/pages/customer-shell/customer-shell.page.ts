import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { CartService } from '../../../../core/services/cart.service';
import { CartDrawerComponent } from '../../components/cart-drawer/cart-drawer.component';

@Component({
  selector: 'app-customer-shell',
  imports: [RouterLink, RouterLinkActive, RouterOutlet, CartDrawerComponent],
  templateUrl: './customer-shell.page.html',
  styleUrl: './customer-shell.page.scss'
})
export class CustomerShellPage {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  readonly cartService = inject(CartService);
  
  readonly user = this.authService.currentUser;
  readonly displayName = computed(() => this.user()?.username || 'ضيف');
  readonly isCashier = computed(() => this.user()?.roles.includes('Cashier') ?? false);
  readonly isAdmin = computed(() => this.user()?.roles.includes('Admin') ?? false);

  readonly brandLink = computed(() => {
    if (this.isCashier()) return '/cashier';
    if (this.isAdmin()) return '/admin';
    return '/customer/menu';
  });

  readonly comingSoonItems = ['العروض', 'تتبع الطلب', 'نقاط الولاء'];
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
