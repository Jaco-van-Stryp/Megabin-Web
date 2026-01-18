import { Component, inject, input, OnInit, signal } from '@angular/core';
import {
  AddressStatus,
  AddressSuggestion,
  AdminService,
  CreateAddress,
  CreateAddressResponse,
  GetUser,
} from '../../../services';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { AutoCompleteCompleteEvent, AutoCompleteModule } from 'primeng/autocomplete';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { Autocomplete } from '../../autocomplete/autocomplete';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-address',
  imports: [
    ButtonModule,
    InputTextModule,
    AutoCompleteModule,
    DialogModule,
    FormsModule,
    Autocomplete,
  ],
  templateUrl: './add-address.html',
})
export class AddAddress implements OnInit {
  adminService = inject(AdminService);
  userId = input.required<string>();
  messageService = inject(MessageService);
  address = signal<CreateAddress>({} as CreateAddress);
  visible = signal(false);
  routerService = inject(Router);

  ngOnInit(): void {
    this.address().userId = this.userId();
  }

  showDialog() {
    this.visible.set(true);
  }

  setAddress(addressSuggestion: AddressSuggestion) {
    this.address().address = addressSuggestion;
  }

  createAddress() {
    this.adminService.apiAdminAddUserAddressPost(this.address()).subscribe({
      next: (response: CreateAddressResponse) => {
        console.log('Address created successfully:', response);
        this.visible.set(false);
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Address created successfully',
        });
        this.routerService.navigate(['/admin/user-addresses', this.userId()]);
      },
      error: (error) => {
        console.error('Error creating address:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to create address',
        });
      },
    });
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
}
