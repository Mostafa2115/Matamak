import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export interface ApiError {
  status: number;
  message: string;
  details?: unknown;
}

export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        authService.clearSession();
        void router.navigate(['/auth/login']);
      }

      return throwError((): ApiError => ({
        status: error.status,
        message: getErrorMessage(error),
        details: error.error
      }));
    })
  );
};

function getErrorMessage(error: HttpErrorResponse): string {
  if (typeof error.error === 'string') {
    return error.error;
  }

  if (error.error && typeof error.error === 'object' && 'message' in error.error) {
    return String(error.error.message);
  }

  return error.message || 'حدث خطأ غير متوقع. حاول مرة أخرى.';
}
