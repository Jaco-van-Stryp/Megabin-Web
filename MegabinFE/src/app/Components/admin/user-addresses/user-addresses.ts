import { Component, inject, input, OnInit, signal } from '@angular/core';
import { AdminService, GetAddress } from '../../../services';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { Button } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { ManageAddress } from '../manage-address/manage-address';

@Component({
  selector: 'app-user-addresses',
  imports: [
    TableModule,
    TagModule,
    InputTextModule,
    IconFieldModule,
    InputIconModule,
    Button,
    TooltipModule,
    ManageAddress,
  ],
  templateUrl: './user-addresses.html',
})
export class UserAddresses implements OnInit {
  adminService = inject(AdminService);
  addresses = signal<GetAddress[]>([]);
  userId = input.required<string>();
  ngOnInit(): void {
    this.loadAddresses();
  }

  loadAddresses() {
    this.adminService.apiAdminGetAllUserAddressesUserIdGet(this.userId()).subscribe({
      next: (response: GetAddress[]) => {
        this.addresses.set(response);
      },
      error: (error) => {
        console.error('Error fetching addresses:', error);
      },
    });
  }

  clickAddress(addressId: string) {
    console.log('Address clicked:', addressId);
  }
}
