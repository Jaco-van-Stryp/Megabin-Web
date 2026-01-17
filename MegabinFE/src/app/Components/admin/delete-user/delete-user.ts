import { Component, inject, input, signal } from '@angular/core';
import { AdminService, GetUser } from '../../../services';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-delete-user',
  imports: [ButtonModule, DialogModule],
  templateUrl: './delete-user.html',
})
export class DeleteUser {
  adminService = inject(AdminService);
  messageService = inject(MessageService);
  routerService = inject(Router);
  visible = signal<boolean>(false);
  User = input.required<GetUser>();

  showDialog() {
    this.visible.set(true);
  }

  deleteUser() {
    this.adminService.apiAdminDeleteUserUserIdDelete(this.User().id).subscribe({
      next: () => {
        this.visible.set(false);
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'User deleted successfully',
        });
        this.routerService.navigate(['/admin/admin-dashboard']);
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to delete user',
        });
        console.error('Error deleting user:', error);
      },
    });
  }
}
