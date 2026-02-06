import {
  Component,
  ChangeDetectionStrategy,
  inject,
  OnInit,
  output,
  signal,
  computed,
} from '@angular/core';
import { AdminService, GetUser } from '../../../services';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { CardModule } from 'primeng/card';
import { ResponsiveService } from '../../../shared/services/responsive.service';

@Component({
  selector: 'app-user-table',
  imports: [TableModule, TagModule, InputTextModule, IconFieldModule, InputIconModule, CardModule],
  templateUrl: './user-table.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserTable implements OnInit {
  private adminService = inject(AdminService);
  private responsiveService = inject(ResponsiveService);

  users = signal<GetUser[]>([]);
  searchTerm = signal('');
  userIdClicked = output<string>();

  readonly isMobile = this.responsiveService.isMobile;

  readonly filteredUsers = computed(() => {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.users();
    return this.users().filter(
      (user) =>
        user.name?.toLowerCase().includes(term) ||
        user.email?.toLowerCase().includes(term) ||
        user.role?.toLowerCase().includes(term),
    );
  });

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.adminService.apiAdminGetAllUsersGet().subscribe({
      next: (response: GetUser[]) => {
        this.users.set(response);
      },
      error: (error) => {
        console.error('Error fetching users:', error);
      },
    });
  }

  clickUser(userId: string): void {
    this.userIdClicked.emit(userId);
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm.set(target.value);
  }

  getRoleSeverity(role: string): 'danger' | 'success' | 'info' | 'warn' | 'secondary' | 'contrast' {
    switch (role) {
      case 'admin':
        return 'danger';
      case 'driver':
        return 'success';
      case 'customer':
        return 'info';
      default:
        return 'secondary';
    }
  }

  getRoleLabel(role: string): string {
    return role.charAt(0).toUpperCase() + role.slice(1);
  }
}
