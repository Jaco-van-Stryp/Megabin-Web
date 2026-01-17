import { Component, inject, OnInit, output, signal } from '@angular/core';
import { AdminService, GetUser } from '../../../services';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';

@Component({
  selector: 'app-user-table',
  imports: [TableModule, TagModule, InputTextModule, IconFieldModule, InputIconModule],
  templateUrl: './user-table.html',
})
export class UserTable implements OnInit {
  adminService = inject(AdminService);
  users = signal<GetUser[]>([]);
  userIdClicked = output<string>();

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.adminService.apiAdminGetAllUsersGet().subscribe({
      next: (response: GetUser[]) => {
        this.users.set(response);
      },
      error: (error) => {
        console.error('Error fetching users:', error);
      },
    });
  }
  clickUser(userId: string) {
    this.userIdClicked.emit(userId);
  }
}
