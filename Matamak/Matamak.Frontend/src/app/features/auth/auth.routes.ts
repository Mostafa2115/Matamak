import { Routes } from '@angular/router';
import { guestGuard } from '../../core/guards/guest.guard';
import { ActivateAccountPage } from './pages/activate-account/activate-account.page';
import { AuthShellPage } from './pages/auth-shell/auth-shell.page';
import { ForgotPasswordPage } from './pages/forgot-password/forgot-password.page';
import { LoginPage } from './pages/login/login.page';
import { RegisterPage } from './pages/register/register.page';
import { ResetPasswordPage } from './pages/reset-password/reset-password.page';

export const authRoutes: Routes = [
  {
    path: '',
    component: AuthShellPage,
    canActivate: [guestGuard],
    children: [
      { path: 'login', component: LoginPage, title: 'تسجيل الدخول | مطعمك' },
      { path: 'register', component: RegisterPage, title: 'إنشاء حساب | مطعمك' },
      { path: 'activate', component: ActivateAccountPage, title: 'تفعيل الحساب | مطعمك' },
      { path: 'forgot-password', component: ForgotPasswordPage, title: 'نسيت كلمة المرور | مطعمك' },
      { path: 'reset-password', component: ResetPasswordPage, title: 'إعادة تعيين كلمة المرور | مطعمك' },
      { path: '', pathMatch: 'full', redirectTo: 'login' }
    ]
  }
];
