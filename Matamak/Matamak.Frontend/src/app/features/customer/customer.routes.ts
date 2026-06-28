import { Routes } from '@angular/router';
import { CustomerShellPage } from './pages/customer-shell/customer-shell.page';
import { ItemDetailPage } from './pages/item-detail/item-detail.page';
import { MenuPage } from './pages/menu/menu.page';
import { CheckoutPage } from './pages/checkout/checkout.page';
import { ProfilePage } from './pages/profile/profile.page';

export const customerRoutes: Routes = [
  {
    path: '',
    component: CustomerShellPage,
    children: [
      { path: 'menu', component: MenuPage, title: 'القائمة | مطعمك' },
      { path: 'items/:id', component: ItemDetailPage, title: 'تفاصيل الصنف | مطعمك' },
      { path: 'checkout', component: CheckoutPage, title: 'إتمام الطلب | مطعمك' },
      { path: 'profile', component: ProfilePage, title: 'الملف الشخصي | مطعمك' },
      { path: '', pathMatch: 'full', redirectTo: 'menu' }
    ]
  }
];
