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
      password: this.password()
    };

    this.authService.apiAuthLoginPost(loginRequest, 'response').subscribe({
      next: (response) => {
        const token = response.headers.get('Authorization')?.replace('Bearer ', '');

        if (token) {
          this.authTokenService.setToken(token);

          this.authService.apiAuthMeGet().subscribe({
            next: (user) => {
              this.authTokenService.setUser(user);
              this.router.navigate(['/']);
            },
            error: (err) => {
              console.error('Failed to fetch user info:', err);
              this.errorMessage.set('Failed to fetch user information');
              this.isLoading.set(false);
            }
          });
        } else {
          this.errorMessage.set('No authentication token received');
          this.isLoading.set(false);
        }
      },
      error: (err) => {
        console.error('Login failed:', err);
        this.errorMessage.set(err.error?.message || 'Invalid email or password');
        this.isLoading.set(false);
      }
    });
  }
}
