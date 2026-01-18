import { Component, computed, inject, input, signal } from '@angular/core';
import {
  AddressStatus,
  AddressSuggestion,
  AdminService,
  GetAddress,
  UpdateAddress,
} from '../../../services';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { AutoCompleteCompleteEvent, AutoCompleteModule } from 'primeng/autocomplete';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { Autocomplete } from '../../autocomplete/autocomplete';

@Component({
  selector: 'app-manage-address',
  imports: [
    ButtonModule,
    InputTextModule,
    AutoCompleteModule,
    DialogModule,
    FormsModule,
    Autocomplete,
  ],
  templateUrl: './manage-address.html',
})
export class ManageAddress {
  address = input.required<GetAddress>();
  adminService = inject(AdminService);
  messageService = inject(MessageService);
  routerService = inject(Router);
  visible = signal(false);
  isReadyForDelivery = computed<boolean>(() => {
    return (
      this.address().addressStatus === AddressStatus.BinDelivered ||
      this.address().addressStatus === AddressStatus.PendingBinDelivery
    );
  });

  showDialog() {
    this.visible.set(true);
  }

  allStatus = [
    { label: 'Pending Address Completion', value: AddressStatus.PendingAddressCompletion },
    { label: 'Pending Bin Payment', value: AddressStatus.PendingBinPayment },
    { label: 'Pending Bin Delivery', value: AddressStatus.PendingBinDelivery },
    { label: 'Bin Requested', value: AddressStatus.BinRequested },
    { label: 'Bin Delivered', value: AddressStatus.BinDelivered },
  ];

  filteredStatus = signal(this.allStatus);

  filterStatus(event: AutoCompleteCompleteEvent) {
    const query = event.query.toLowerCase();
    this.filteredStatus.set(
      this.allStatus.filter((status) => status.label.toLowerCase().includes(query)),
    );
  }

  setAddress(address: AddressSuggestion) {
    this.address().address = address.label ?? '';
    this.address().location.latitude = address.location?.latitude;
    this.address().location.longitude = address.location?.longitude;
  }

  updateAddressDetails() {
    const updateAddress: UpdateAddress = {
      addressId: this.address().id,
      totalBins: this.address().totalBins,
      addressNotes: this.address().addressNotes,
      status: this.address().addressStatus,
      location: this.address().location,
    };
    this.adminService.apiAdminUpdateUserAddressPost(updateAddress).subscribe({
      next: () => {
        this.visible.set(false);
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Address updated successfully',
        });
      },
      error: (error) => {
        console.error('Error updating address:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to update address',
        });
      },
    });
  }

  manageScheduleContracts() {
    this.routerService.navigate(['/admin/manage-schedule-contracts', this.address().id]);
  }
}
