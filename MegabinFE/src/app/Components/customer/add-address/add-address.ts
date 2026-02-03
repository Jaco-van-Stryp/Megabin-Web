import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { TextareaModule } from 'primeng/textarea';
import { CardModule } from 'primeng/card';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { Autocomplete } from '../../autocomplete/autocomplete';
import {
  CustomerService,
  CustomerCreateAddressCommand,
  AddressSuggestion,
} from '../../../services';

@Component({
  selector: 'app-customer-add-address',
  imports: [
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    TextareaModule,
    CardModule,
    ToastModule,
    Autocomplete,
  ],
  providers: [MessageService],
  templateUrl: './add-address.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CustomerAddAddress {
  private customerService = inject(CustomerService);
  private router = inject(Router);
  private messageService = inject(MessageService);

  selectedAddress = signal<AddressSuggestion | null>(null);
  totalBins = signal<number>(1);
  addressNotes = signal<string>('');
  isSubmitting = signal<boolean>(false);

  onAddressSelected(address: AddressSuggestion): void {
    this.selectedAddress.set(address);
  }

  createAddress(): void {
    const address = this.selectedAddress();
    if (!address) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Please select an address',
      });
      return;
    }

    this.isSubmitting.set(true);

    const command: CustomerCreateAddressCommand = {
      address: address,
      totalBins: this.totalBins(),
      addressNotes: this.addressNotes() || undefined,
    };

    this.customerService.apiCustomerCreateAddressPost(command).subscribe({
      next: (response) => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Address created successfully',
        });
        this.router.navigate(['/customer/addresses', response.addressId]);
      },
      error: (err) => {
        console.error('Error creating address:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to create address',
        });
        this.isSubmitting.set(false);
      },
    });
  }

  goBack(): void {
    this.router.navigate(['/customer']);
  }
}
