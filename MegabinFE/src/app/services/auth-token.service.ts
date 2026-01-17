import { Injectable, signal } from '@angular/core';
import { GetUser } from './model/getUser';

/**
 * Service for managing authentication tokens and user state
 */
@Injectable({
  providedIn: 'root'
})
export class AuthTokenService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'user_info';

  private userSignal = signal<GetUser | null>(this.loadUserFromStorage());
  private tokenSignal = signal<string | null>(this.loadTokenFromStorage());

  readonly user = this.userSignal.asReadonly();
  readonly token = this.tokenSignal.asReadonly();
  readonly isAuthenticated = signal(this.loadTokenFromStorage() !== null);

  private loadTokenFromStorage(): string | null {
    if (typeof window !== 'undefined' && window.localStorage) {
      return localStorage.getItem(this.TOKEN_KEY);
    }
    return null;
  }

  private loadUserFromStorage(): GetUser | null {
    if (typeof window !== 'undefined' && window.localStorage) {
      const user = localStorage.getItem(this.USER_KEY);
      return user ? JSON.parse(user) : null;
    }
    return null;
  }

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    this.tokenSignal.set(token);
    this.isAuthenticated.set(true);
  }

  getToken(): string | null {
    return this.tokenSignal();
  }

  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.tokenSignal.set(null);
    this.isAuthenticated.set(false);
  }

  setUser(user: GetUser): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.userSignal.set(user);
  }

  getUser(): GetUser | null {
    return this.userSignal();
  }

  removeUser(): void {
    localStorage.removeItem(this.USER_KEY);
    this.userSignal.set(null);
  }

  logout(): void {
    this.removeToken();
    this.removeUser();
  }
}
