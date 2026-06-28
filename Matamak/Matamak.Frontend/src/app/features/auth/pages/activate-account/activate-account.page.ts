import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { ApiError } from '../../../../core/interceptors/error.interceptor';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-activate-account-page',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './activate-account.page.html',
  styleUrl: './activate-account.page.scss'
})
export class ActivateAccountPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  readonly isSubmitting = signal(false);
  readonly statusMessage = signal<string | null>(null);
  readonly errorMessage = signal<string | null>(null);
  readonly form = this.formBuilder.nonNullable.group({
    email: [this.route.snapshot.queryParamMap.get('email') ?? '', [Validators.required, Validators.email]],
    code: ['', [Validators.required, Validators.pattern(/^\d+$/)]]
  });

  submit(): void {
    this.form.markAllAsTouched();
    if (!this.form.valid || this.isSubmitting()) {
      return;
    }

    this.errorMessage.set(null);
    this.statusMessage.set(null);
    this.isSubmitting.set(true);

    const value = this.form.getRawValue();
    this.authService
      .activateAccount(value.email, Number(value.code))
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (message) => {
          this.statusMessage.set(message);
          setTimeout(() => void this.router.navigate(['/auth/login']), 900);
        },
        error: (error: ApiError) => this.errorMessage.set(error.message)
      });
  }
}
