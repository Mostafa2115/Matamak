import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then((routes) => routes.authRoutes)
  },
  {
    path: 'customer',
    canActivate: [authGuard],
    loadChildren: () => import('./features/customer/customer.routes').then((routes) => routes.customerRoutes)
  },
  {
    path: 'cashier',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Cashier'], role: 'Cashier' },
    loadComponent: () => import('./features/staff/staff-dashboard.page').then((page) => page.StaffDashboardPage),
    title: 'واجهة الكاشير | مطعمك'
  },
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'], role: 'Admin' },
    loadComponent: () => import('./features/staff/staff-dashboard.page').then((page) => page.StaffDashboardPage),
    title: 'لوحة الإدارة | مطعمك'
  },
  { path: '', pathMatch: 'full', redirectTo: 'auth/login' },
  { path: '**', redirectTo: 'auth/login' }
];
