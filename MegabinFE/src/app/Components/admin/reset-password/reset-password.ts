import { Component, inject, input, signal } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { AdminService, GetUser } from '../../../services';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
@Component({
  selector: 'app-reset-password',
  imports: [DialogModule, ButtonModule, InputTextModule, FormsModule],
  templateUrl: './reset-password.html',
})
export class ResetPassword {
  adminService = inject(AdminService);
  messageService = inject(MessageService);
  visible = signal<boolean>(false);
  User = input.required<GetUser>();
  password = '';

  showDialog() {
    this.visible.set(true);
  }

  resetPassword() {
    this.adminService
      .apiAdminResetUserPasswordPost({
        userId: this.User().id,
        newPassword: this.password,
      })
      .subscribe({
        next: () => {
          this.visible.set(false);
          this.password = '';
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Password reset successfully',
          });
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to reset password',
          });
          console.error('Error resetting password:', error);
        },
      });
  }
}
