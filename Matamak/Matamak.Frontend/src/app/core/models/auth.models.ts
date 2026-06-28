export type UserRole = 'Admin' | 'Cashier' | 'Customer';

export interface RegisterRequest {
  username: string;
  fullName: string;
  address: string;
  email: string;
  phone: string;
  password: string;
  confirmPassword: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginTokenResponse {
  loginToken: string;
  refreshToken: string;
  tokenExpiration: string;
  refreshTokenExpiration: string;
}

export interface RefreshTokenRequest {
  token: string;
  refreshToken: string;
}

export interface EditAccountRequest {
  FullName: string;
  Address: string;
  username: string;
  phonenumber: string;
}

export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ResetPasswordRequest {
  NewPassword: string;
  ConfirmPassword: string;
}

export interface AuthUser {
  username: string;
  email: string;
  roles: UserRole[];
  tokenExpiration: string;
}

export interface JwtPayload {
  nameid?: string;
  unique_name?: string;
  email?: string;
  role?: string | string[];
  exp?: number;
  [claim: string]: unknown;
}
