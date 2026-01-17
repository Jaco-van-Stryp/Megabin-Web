import { Injectable, signal, computed } from '@angular/core';
import { LoginResponse } from './model/loginResponse';

/**
 * Service for managing authentication tokens and user state
 */
@Injectable({
  providedIn: 'root'
})
export class AuthTokenService {
  private readonly AUTH_KEY = 'auth_data';

  private authDataSignal = signal<LoginResponse | null>(this.loadAuthFromStorage());

  readonly authData = this.authDataSignal.asReadonly();
  readonly user = computed(() => {
    const auth = this.authDataSignal();
    if (!auth) return null;
    return {
      userId: auth.userId,
      name: auth.name,
      email: auth.email,
      role: auth.role
    };
  });
  readonly token = computed(() => this.authDataSignal()?.token ?? null);
  readonly isAuthenticated = computed(() => this.authDataSignal()?.token != null);

  private loadAuthFromStorage(): LoginResponse | null {
    if (typeof window !== 'undefined' && window.localStorage) {
      const auth = localStorage.getItem(this.AUTH_KEY);
      return auth ? JSON.parse(auth) : null;
    }
    return null;
  }

  setAuthData(loginResponse: LoginResponse): void {
    localStorage.setItem(this.AUTH_KEY, JSON.stringify(loginResponse));
    this.authDataSignal.set(loginResponse);
  }

  getToken(): string | null {
    return this.token();
  }

  logout(): void {
    localStorage.removeItem(this.AUTH_KEY);
    this.authDataSignal.set(null);
  }
}
