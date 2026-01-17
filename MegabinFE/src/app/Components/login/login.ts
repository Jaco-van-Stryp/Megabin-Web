import { Component, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { RippleModule } from 'primeng/ripple';
import { AuthService } from '../../services/api/auth.service';
import { AuthTokenService } from '../../services/auth-token.service';
import { LoginRequest } from '../../services/model/loginRequest';
import { LoginResponse } from '../../services';

@Component({
  selector: 'app-login',
  imports: [
    ButtonModule,
    CheckboxModule,
    InputTextModule,
    PasswordModule,
    FormsModule,
    RouterModule,
    RippleModule,
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Login {
  private readonly authService = inject(AuthService);
  private readonly authTokenService = inject(AuthTokenService);
  private readonly router = inject(Router);

  email = signal('');
  password = signal('');
  rememberMe = signal(false);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  onLogin(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const loginRequest: LoginRequest = {
      email: this.email(),
      password: this.password(),
    };

    this.authService.apiAuthLoginPost(loginRequest).subscribe({
      next: (loginResponse: LoginResponse) => {
        if (loginResponse.token && loginResponse.userId) {
          this.authTokenService.setAuthData(loginResponse);
          this.isLoading.set(false);

          // Redirect based on user role
          // UserRoles: 0 = Customer, 1 = Driver, 2 = Admin
          if (loginResponse.role === 2) {
            this.router.navigate(['/admin']);
          } else if (loginResponse.role === 1) {
            // TODO: Driver dashboard not yet implemented
            this.router.navigate(['/autocomplete']);
          } else {
            // Customer
            // TODO: Customer dashboard not yet implemented
            this.router.navigate(['/autocomplete']);
          }
        } else {
          this.errorMessage.set('Invalid response from server');
          this.isLoading.set(false);
        }
      },
      error: (err) => {
        console.error('Login failed:', err);
        this.errorMessage.set(err.error?.message || 'Invalid email or password');
        this.isLoading.set(false);
      },
    });
  }
}
