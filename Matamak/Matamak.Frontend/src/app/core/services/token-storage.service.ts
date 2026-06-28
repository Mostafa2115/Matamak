import { Injectable, signal } from '@angular/core';
import { LoginTokenResponse } from '../models/auth.models';

const ACCESS_TOKEN_KEY = 'matamak.accessToken';
const REFRESH_TOKEN_KEY = 'matamak.refreshToken';
const TOKEN_EXPIRATION_KEY = 'matamak.tokenExpiration';
const REFRESH_EXPIRATION_KEY = 'matamak.refreshTokenExpiration';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  readonly tokenChanged = signal<string | null>(this.getAccessToken());

  saveToken(token: LoginTokenResponse): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, token.loginToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, token.refreshToken);
    localStorage.setItem(TOKEN_EXPIRATION_KEY, token.tokenExpiration);
    localStorage.setItem(REFRESH_EXPIRATION_KEY, token.refreshTokenExpiration);
    this.tokenChanged.set(token.loginToken);
  }

  clear(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(TOKEN_EXPIRATION_KEY);
    localStorage.removeItem(REFRESH_EXPIRATION_KEY);
    this.tokenChanged.set(null);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  getTokenExpiration(): string | null {
    return localStorage.getItem(TOKEN_EXPIRATION_KEY);
  }
}
