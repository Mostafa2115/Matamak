import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { API_PATHS } from '../constants/api-paths';
import {
  AuthUser,
  ChangePasswordRequest,
  EditAccountRequest,
  JwtPayload,
  LoginRequest,
  LoginTokenResponse,
  RefreshTokenRequest,
  RegisterRequest,
  ResetPasswordRequest,
  UserRole
} from '../models/auth.models';
import { CustomerUser } from '../models/user.models';
import { ApiUrlService } from './api-url.service';
import { TokenStorageService } from './token-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(ApiUrlService);
  private readonly tokenStorage = inject(TokenStorageService);
  private readonly currentUserSignal = signal<AuthUser | null>(this.createUserFromStoredToken());

  readonly currentUser = this.currentUserSignal.asReadonly();
  readonly isAuthenticated = computed(() => this.currentUserSignal() !== null);

  register(request: RegisterRequest): Observable<string> {
    return this.http.post(this.apiUrl.build(API_PATHS.auth.register), request, { responseType: 'text' });
  }

  activateAccount(email: string, code: number): Observable<string> {
    return this.http.post(this.apiUrl.build(API_PATHS.auth.activateAccount(email)), code, { responseType: 'text' });
  }

  login(request: LoginRequest): Observable<AuthUser> {
    return this.http.post<LoginTokenResponse>(this.apiUrl.build(API_PATHS.auth.login), request).pipe(
      tap((token) => this.tokenStorage.saveToken(token)),
      map((token) => this.createUserFromToken(token.loginToken, token.tokenExpiration)),
      tap((user) => this.currentUserSignal.set(user))
    );
  }

  refreshToken(): Observable<LoginTokenResponse> {
    const accessToken = this.tokenStorage.getAccessToken();
    const refreshToken = this.tokenStorage.getRefreshToken();

    if (!accessToken || !refreshToken) {
      return throwError(() => new Error('Missing authentication token.'));
    }

    const request: RefreshTokenRequest = {
      token: accessToken,
      refreshToken
    };

    return this.http.post<LoginTokenResponse>(this.apiUrl.build(API_PATHS.auth.refreshToken), request).pipe(
      tap((token) => {
        this.tokenStorage.saveToken(token);
        this.currentUserSignal.set(this.createUserFromToken(token.loginToken, token.tokenExpiration));
      })
    );
  }

  logout(): Observable<string> {
    return this.http.post(this.apiUrl.build(API_PATHS.auth.logout), null, { responseType: 'text' }).pipe(
      catchError((error) => {
        this.clearSession();
        return throwError(() => error);
      }),
      tap(() => this.clearSession())
    );
  }

  clearSession(): void {
    this.tokenStorage.clear();
    this.currentUserSignal.set(null);
  }

  editAccount(username: string, request: EditAccountRequest): Observable<string> {
    return this.http.put(this.apiUrl.build(API_PATHS.auth.editAccount(username)), request, { responseType: 'text' });
  }

  changePassword(username: string, request: ChangePasswordRequest): Observable<string> {
    return this.http.put(this.apiUrl.build(API_PATHS.auth.changePassword(username)), request, {
      responseType: 'text'
    });
  }

  forgotPassword(email: string): Observable<string> {
    return this.http.post(this.apiUrl.build(API_PATHS.auth.forgotPassword), JSON.stringify(email), {
      headers: { 'Content-Type': 'application/json' },
      responseType: 'text'
    });
  }

  verifyForgotPasswordCode(email: string, code: number): Observable<string> {
    return this.http.post(this.apiUrl.build(API_PATHS.auth.verifyForgotPasswordCode(email)), code, {
      responseType: 'text'
    });
  }

  resetPassword(email: string, request: ResetPasswordRequest): Observable<string> {
    return this.http.put(this.apiUrl.build(API_PATHS.auth.resetPassword(email)), request, { responseType: 'text' });
  }

  getCustomer(username: string): Observable<CustomerUser> {
    return this.http.get<CustomerUser>(this.apiUrl.build(API_PATHS.auth.getCustomerByUsername(username)));
  }

  getAllAdmins(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.build(API_PATHS.auth.getAllAdmins));
  }

  getAllCashiers(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.build(API_PATHS.auth.getAllCashiers));
  }

  getAllCustomers(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.build(API_PATHS.auth.getAllCustomers));
  }

  registerStaff(request: RegisterRequest, role: string): Observable<string> {
    return this.http.post(
      this.apiUrl.build(`${API_PATHS.auth.staffRegister}?role=${encodeURIComponent(role)}`),
      request,
      { responseType: 'text' }
    );
  }

  deleteAnyAccount(username: string): Observable<string> {
    return this.http.delete(
      this.apiUrl.build(API_PATHS.auth.deleteAnyAccount(username)),
      { responseType: 'text' }
    );
  }

  hasRole(roles: UserRole[]): boolean {
    const user = this.currentUserSignal();
    return roles.some((role) => user?.roles.includes(role));
  }

  private createUserFromStoredToken(): AuthUser | null {
    const token = this.tokenStorage.getAccessToken();
    const expiration = this.tokenStorage.getTokenExpiration();
    if (!token || !expiration) {
      return null;
    }

    if (this.isExpired(expiration)) {
      this.tokenStorage.clear();
      return null;
    }

    try {
      return this.createUserFromToken(token, expiration);
    } catch {
      this.tokenStorage.clear();
      return null;
    }
  }

  private createUserFromToken(token: string, tokenExpiration: string): AuthUser {
    const payload = this.decodeJwt(token);
    const roleClaim =
      payload.role ??
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    const roles = Array.isArray(roleClaim) ? roleClaim : roleClaim ? [roleClaim] : [];

    return {
      username: String(payload.unique_name ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ?? ''),
      email: String(payload.email ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? ''),
      roles: roles.filter(this.isUserRole),
      tokenExpiration
    };
  }

  private decodeJwt(token: string): JwtPayload {
    const payload = token.split('.')[1];
    if (!payload) {
      return {};
    }

    const normalizedPayload = payload.replace(/-/g, '+').replace(/_/g, '/');
    const decoded = atob(normalizedPayload);
    return JSON.parse(decoded) as JwtPayload;
  }

  private isUserRole(role: string): role is UserRole {
    return role === 'Admin' || role === 'Cashier' || role === 'Customer';
  }

  private isExpired(expiration: string): boolean {
    const expiresAt = Date.parse(expiration);
    return Number.isFinite(expiresAt) && expiresAt <= Date.now();
  }
}
