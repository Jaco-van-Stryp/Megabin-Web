import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { SkeletonModule } from 'primeng/skeleton';
import { AdminStateService } from '../../../services/admin-state.service';
import { GetAddress } from '../../../services/model/getAddress';
import { AddressStatus } from '../../../services/model/addressStatus';
import { AddAddressDialog } from '../dialogs/add-address-dialog/add-address-dialog';
import { AddressSuggestion } from '../../../services/model/addressSuggestion';

@Component({
  selector: 'app-user-detail',
  imports: [CommonModule, CardModule, ButtonModule, TagModule, SkeletonModule, AddAddressDialog],
  templateUrl: './user-detail.html',
  styleUrls: ['./user-detail.css'],
})
export class UserDetail implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  adminState = inject(AdminStateService);

  userId = signal<string>('');
  showAddAddressDialog = signal<boolean>(false);

  currentUser = computed(() => this.adminState.selectedUser());

  ngOnInit(): void {
    const userIdParam = this.route.snapshot.paramMap.get('userId');
    if (userIdParam) {
      this.userId.set(userIdParam);
      this.adminState.loadUserAddresses(userIdParam);
    } else {
      this.router.navigate(['/admin/users']);
    }
  }

  goBack(): void {
    this.router.navigate(['/admin/users']);
  }

  onAddressClick(address: GetAddress): void {
    this.router.navigate(['/admin/users', this.userId(), 'addresses', address.id]);
  }

  async onSaveAddress(data: {
    address: AddressSuggestion;
    totalBins: number;
    addressNotes: string;
  }): Promise<void> {
    const addressId = await this.adminState.addAddress({
      userId: this.userId(),
      address: data.address,
      totalBins: data.totalBins,
      addressNotes: data.addressNotes,
    });

    if (addressId) {
      this.showAddAddressDialog.set(false);
      // Navigate to the newly created address
      this.router.navigate(['/admin/users', this.userId(), 'addresses', addressId]);
    }
  }

  getStatusLabel(status: AddressStatus): string {
    const labels = {
      [AddressStatus.NUMBER_0]: 'Pending Completion',
      [AddressStatus.NUMBER_1]: 'Bin Requested',
      [AddressStatus.NUMBER_2]: 'Pending Payment',
      [AddressStatus.NUMBER_3]: 'Pending Delivery',
      [AddressStatus.NUMBER_4]: 'Bin Delivered',
    };
    return labels[status] || 'Unknown';
  }

  getStatusSeverity(status: AddressStatus): 'success' | 'info' | 'warn' | 'secondary' {
    const severities = {
      [AddressStatus.NUMBER_0]: 'secondary' as const,
      [AddressStatus.NUMBER_1]: 'info' as const,
      [AddressStatus.NUMBER_2]: 'warn' as const,
      [AddressStatus.NUMBER_3]: 'warn' as const,
      [AddressStatus.NUMBER_4]: 'success' as const,
    };
    return severities[status] || 'secondary';
  }
}
