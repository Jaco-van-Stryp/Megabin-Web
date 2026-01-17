import { Component, inject, input, OnInit, signal } from '@angular/core';
import { UserAddresses } from '../user-addresses/user-addresses';
import { AdminService, GetUser } from '../../../services';
import { Button } from "primeng/button";

@Component({
  selector: 'app-manage-user',
  imports: [UserAddresses, Button],
  templateUrl: './manage-user.html',
})
export class ManageUser implements OnInit {
  userId = input.required<string>();
  userService = inject(AdminService);
  user = signal<GetUser>({} as GetUser);

  ngOnInit(): void {
    this.loadUserDetails();
  }

  loadUserDetails() {
    this.userService.apiAdminGetUserUserIdGet(this.userId()).subscribe({
      next: (response: GetUser) => {
        this.user.set(response);
      },
      error: (error) => {
        console.error('Error fetching user details:', error);
      },
    });
  }
}
