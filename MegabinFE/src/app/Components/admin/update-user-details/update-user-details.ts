import { Component, inject, input, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { AutoCompleteCompleteEvent, AutoCompleteModule } from 'primeng/autocomplete';
import { AdminService, GetUser, UpdateUser, UserRoles } from '../../../services';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-update-user-details',
  imports: [DialogModule, ButtonModule, InputTextModule, FormsModule, AutoCompleteModule],
  templateUrl: './update-user-details.html',
})
export class UpdateUserDetails {
  adminService = inject(AdminService);
  messageService = inject(MessageService);
  visible = signal<boolean>(false);
  User = input.required<GetUser>();

  allRoles = [
    { label: 'Admin', value: UserRoles.Admin },
    { label: 'Driver', value: UserRoles.Driver },
    { label: 'Customer', value: UserRoles.Customer },
  ];

  filteredRoles = signal(this.allRoles);

  showDialog() {
    this.visible.set(true);
  }
  filterRoles(event: AutoCompleteCompleteEvent) {
    const query = event.query.toLowerCase();
    this.filteredRoles.set(
      this.allRoles.filter((role) => role.label.toLowerCase().includes(query)),
    );
  }
  updateUserDetails() {
    var updateRequest: UpdateUser = {
      userId: this.User().id,
      name: this.User().name,
      email: this.User().email,
      phoneNumber: this.User().phoneNumber,
      role: this.User().role,
    };
    this.adminService.apiAdminUpdateUserPost(updateRequest).subscribe({
      next: () => {
        this.visible.set(false);
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'User details updated successfully',
        });
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to update user details',
        });
        console.error('Error updating user details:', error);
      },
    });
  }
}
