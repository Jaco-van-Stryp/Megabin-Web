import { Component, ChangeDetectionStrategy, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { AuthService, CustomerService, ProfileResponse } from '../../../services';

@Component({
  selector: 'app-edit-profile',
  imports: [FormsModule, CardModule, ButtonModule, InputTextModule, ToastModule],
  providers: [MessageService],
  templateUrl: './edit-profile.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EditProfile implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthService);
  private customerService = inject(CustomerService);
  private messageService = inject(MessageService);

  profile = signal<ProfileResponse | null>(null);
  name = signal<string>('');
  phoneNumber = signal<string>('');
  isLoading = signal<boolean>(true);
  isSubmitting = signal<boolean>(false);

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.authService.apiAuthMeGet().subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error loading profile:', err);
        this.isLoading.set(false);
      },
    });
  }

  updateProfile(): void {
    const nameVal = this.name();
    const phoneVal = this.phoneNumber();

    if (!nameVal.trim() || !phoneVal.trim()) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Name and phone number are required',
      });
      return;
    }

    this.isSubmitting.set(true);

    this.customerService
      .apiCustomerUpdateProfilePatch({
        name: nameVal,
        phoneNumber: phoneVal,
      })
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Profile updated successfully',
          });
          this.isSubmitting.set(false);
        },
        error: (err) => {
          console.error('Error updating profile:', err);
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to update profile',
          });
          this.isSubmitting.set(false);
        },
      });
  }

  goBack(): void {
    this.router.navigate(['/customer']);
  }
}
