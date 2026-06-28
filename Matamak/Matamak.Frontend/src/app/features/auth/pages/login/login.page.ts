import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { ApiError } from '../../../../core/interceptors/error.interceptor';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.page.html',
  styleUrl: './login.page.scss'
})
export class LoginPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });
  canSubmit(): boolean {
    return this.form.valid && !this.isSubmitting();
  }

  fillAdmin(): void {
    this.form.setValue({ email: 'admin@gmail.com', password: '123456789' });
  }

  fillCashier(): void {
    this.form.setValue({ email: 'cashier@gmail.com', password: '147258369' });
  }

  submit(): void {
    this.form.markAllAsTouched();
    if (!this.form.valid || this.isSubmitting()) {
      return;
    }

    this.errorMessage.set(null);
    this.isSubmitting.set(true);

    this.authService
      .login(this.form.getRawValue())
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (user) => {
          const destination = user.roles.includes('Admin')
            ? '/admin'
            : user.roles.includes('Cashier')
              ? '/cashier'
              : '/customer';
          void this.router.navigate([destination]);
        },
        error: (error: ApiError) => this.errorMessage.set(error.message)
      });
  }
}
