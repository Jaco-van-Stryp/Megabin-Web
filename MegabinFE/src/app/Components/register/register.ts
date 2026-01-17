import { Component, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { SelectModule } from 'primeng/select';
import { AuthService } from '../../services/api/auth.service';
import { AuthTokenService } from '../../services/auth-token.service';
import { RegisterRequest } from '../../services/model/registerRequest';
import { UserRoles } from '../../services/model/userRoles';

@Component({
  selector: 'app-register',
  imports: [ButtonModule, InputTextModule, PasswordModule, SelectModule, FormsModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private readonly authService = inject(AuthService);
  private readonly authTokenService = inject(AuthTokenService);
  private readonly router = inject(Router);

  name = signal('');
  email = signal('');
  password = signal('');
  confirmPassword = signal('');
  phoneNumber = signal('');
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  roleOptions = [
    { label: 'Customer', value: UserRoles.Customer },
    { label: 'Driver', value: UserRoles.Driver },
    { label: 'Admin', value: UserRoles.Admin },
  ];

  onRegister(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    // Validation
    if (!this.name() || !this.email() || !this.password() || !this.confirmPassword()) {
      this.errorMessage.set('All fields are required');
      return;
    }

    if (this.password() !== this.confirmPassword()) {
      this.errorMessage.set('Passwords do not match');
      return;
    }

    if (this.password().length < 6) {
      this.errorMessage.set('Password must be at least 6 characters long');
      return;
    }

    this.isLoading.set(true);

    const registerRequest: RegisterRequest = {
      name: this.name(),
      email: this.email(),
      password: this.password(),
      phoneNumber: this.phoneNumber(),
    };

    this.authService.apiAuthRegisterPost(registerRequest).subscribe({
      next: (loginResponse) => {
        if (loginResponse.token && loginResponse.userId) {
          this.authTokenService.setAuthData(loginResponse);
          this.successMessage.set('Registration successful! Redirecting...');
          this.isLoading.set(false);

          // Redirect based on user role
          // UserRoles: 'customer', 'driver', 'admin'
          if (loginResponse.role === 'admin') {
            this.router.navigate(['/admin']);
          } else if (loginResponse.role === 'driver') {
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
        console.error('Registration failed:', err);
        this.errorMessage.set(err.error?.message || 'Registration failed. Please try again.');
        this.isLoading.set(false);
      },
    });
  }
}
