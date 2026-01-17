import {
  Component,
  inject,
  signal,
  computed,
  ChangeDetectionStrategy,
  OnInit,
} from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { SkeletonModule } from 'primeng/skeleton';
import { CardModule } from 'primeng/card';
import { AdminStateService } from '../../../services/admin-state.service';
import { GetUser } from '../../../services/model/getUser';
import { UserRoles } from '../../../services/model/userRoles';
import { EditUserDialog } from '../dialogs/edit-user-dialog/edit-user-dialog';
import { ResetPasswordDialog } from '../dialogs/reset-password-dialog/reset-password-dialog';
import { ConfirmDialog } from '../dialogs/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-user-list',
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    TagModule,
    TooltipModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    SkeletonModule,
    CardModule,
    EditUserDialog,
    ResetPasswordDialog,
    ConfirmDialog,
  ],
  templateUrl: './user-list.html',
  styleUrls: ['./user-list.css'],
})
export class UserList implements OnInit {
  private router = inject(Router);
  adminState = inject(AdminStateService);

  searchQuery = signal<string>('');
  selectedUser = signal<GetUser | null>(null);
  showEditDialog = signal<boolean>(false);
  showResetPasswordDialog = signal<boolean>(false);
  showDeleteDialog = signal<boolean>(false);
  userToDelete = signal<GetUser | null>(null);

  // Filtered users based on search
  filteredUsers = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.adminState.users();

    return this.adminState
      .users()
      .filter(
        (user) =>
          user.name?.toLowerCase().includes(query) ||
          user.email?.toLowerCase().includes(query) ||
          this.getRoleLabel(user.role).toLowerCase().includes(query),
      );
  });

  ngOnInit(): void {
    this.adminState.loadUsers();
  }

  onRowClick(user: GetUser): void {
    this.router.navigate(['/admin/users', user.id]);
  }

  onEditUser(user: GetUser, event: Event): void {
    event.stopPropagation();
    this.selectedUser.set(user);
    this.showEditDialog.set(true);
  }

  onResetPassword(user: GetUser, event: Event): void {
    event.stopPropagation();
    this.selectedUser.set(user);
    this.showResetPasswordDialog.set(true);
  }

  onDeleteUser(user: GetUser, event: Event): void {
    event.stopPropagation();
    this.userToDelete.set(user);
    this.showDeleteDialog.set(true);
  }

  async onSaveUser(data: {
    name: string;
    email: string;
    role: UserRoles;
    totalBins: number;
  }): Promise<void> {
    const user = this.selectedUser();
    if (!user) return;

    const success = await this.adminState.updateUser({
      userId: user.id,
      name: data.name,
      email: data.email,
      role: data.role,
      totalBins: data.totalBins,
    });

    if (success) {
      this.showEditDialog.set(false);
      this.selectedUser.set(null);
    }
  }

  async onSavePassword(newPassword: string): Promise<void> {
    const user = this.selectedUser();
    if (!user) return;

    const success = await this.adminState.resetPassword(user.id, newPassword);

    if (success) {
      this.showResetPasswordDialog.set(false);
      this.selectedUser.set(null);
    }
  }

  async confirmDelete(): Promise<void> {
    const user = this.userToDelete();
    if (!user) return;

    const success = await this.adminState.deleteUser(user.id);

    if (success) {
      this.showDeleteDialog.set(false);
      this.userToDelete.set(null);
    }
  }

  getRoleLabel(role: UserRoles): string {
    const labels: { [key: string]: string } = {
      [UserRoles.Customer]: 'Customer',
      [UserRoles.Driver]: 'Driver',
      [UserRoles.Admin]: 'Admin',
    };
    return labels[role] || 'Unknown';
  }

  getRoleSeverity(role: UserRoles): 'success' | 'info' | 'warn' {
    const severities: { [key: string]: 'success' | 'info' | 'warn' } = {
      [UserRoles.Customer]: 'info' as const,
      [UserRoles.Driver]: 'warn' as const,
      [UserRoles.Admin]: 'success' as const,
    };
    return severities[role] || 'info';
  }
}
