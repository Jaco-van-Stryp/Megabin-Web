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
import { LoginResponse, UserRoles } from '../../services';

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
})
export class Login {
  private readonly authService = inject(AuthService);
  private readonly authTokenService = inject(AuthTokenService);
  private readonly router = inject(Router);

  email = signal('');
  password = signal('');
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

          if (loginResponse.role === UserRoles.Admin) {
            this.router.navigate(['/admin/admin-dashboard']);
          } else if (loginResponse.role === UserRoles.Driver) {
            this.router.navigate(['/driver']);
          } else if (loginResponse.role === UserRoles.Customer) {
            this.router.navigate(['/customer']);
          } else {
            this.errorMessage.set('Unknown user role');
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
