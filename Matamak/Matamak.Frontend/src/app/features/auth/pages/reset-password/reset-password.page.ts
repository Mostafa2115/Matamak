import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, switchMap } from 'rxjs';
import { ApiError } from '../../../../core/interceptors/error.interceptor';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-reset-password-page',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './reset-password.page.html',
  styleUrl: './reset-password.page.scss'
})
export class ResetPasswordPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly form = this.formBuilder.nonNullable.group(
    {
      email: [this.route.snapshot.queryParamMap.get('email') ?? '', [Validators.required, Validators.email]],
      code: ['', [Validators.required, Validators.pattern(/^\d+$/)]],
      NewPassword: ['', [Validators.required, Validators.minLength(6)]],
      ConfirmPassword: ['', Validators.required]
    },
    { validators: [resetPasswordsMatchValidator] }
  );

  submit(): void {
    this.form.markAllAsTouched();
    if (!this.form.valid || this.isSubmitting()) {
      return;
    }

    this.errorMessage.set(null);
    this.isSubmitting.set(true);
    const value = this.form.getRawValue();

    this.authService
      .verifyForgotPasswordCode(value.email, Number(value.code))
      .pipe(
        switchMap(() =>
          this.authService.resetPassword(value.email, {
            NewPassword: value.NewPassword,
            ConfirmPassword: value.ConfirmPassword
          })
        ),
        finalize(() => this.isSubmitting.set(false))
      )
      .subscribe({
        next: () => void this.router.navigate(['/auth/login']),
        error: (error: ApiError) => this.errorMessage.set(error.message)
      });
  }
}

function resetPasswordsMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('NewPassword')?.value;
  const confirmPassword = control.get('ConfirmPassword')?.value;
  return password === confirmPassword ? null : { passwordsMismatch: true };
}
