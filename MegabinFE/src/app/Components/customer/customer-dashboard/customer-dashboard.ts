import {
  Component,
  ChangeDetectionStrategy,
  inject,
  OnInit,
  signal,
  computed,
} from '@angular/core';
import { Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { AddressService, GetAddress } from '../../../services';
import { AddressStatus } from '../../../services/model/addressStatus';
import { ResponsiveService } from '../../../shared/services/responsive.service';

@Component({
  selector: 'app-customer-dashboard',
  imports: [
    TableModule,
    TagModule,
    ButtonModule,
    InputTextModule,
    IconFieldModule,
    InputIconModule,
    TooltipModule,
    CardModule,
  ],
  templateUrl: './customer-dashboard.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CustomerDashboard implements OnInit {
  private addressService = inject(AddressService);
  private router = inject(Router);
  private responsiveService = inject(ResponsiveService);

  addresses = signal<GetAddress[]>([]);
  searchTerm = signal('');

  readonly isMobile = this.responsiveService.isMobile;

  readonly filteredAddresses = computed(() => {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.addresses();
    return this.addresses().filter(
      (addr) =>
        addr.address?.toLowerCase().includes(term) ||
        addr.addressStatus?.toLowerCase().includes(term)
    );
  });

  ngOnInit(): void {
    this.loadAddresses();
  }

  loadAddresses(): void {
    this.addressService.apiAddressGetAllAddressesGet().subscribe({
      next: (addresses) => this.addresses.set(addresses),
      error: (err) => console.error('Error loading addresses:', err),
    });
  }

  viewAddress(addressId: string): void {
    this.router.navigate(['/customer/addresses', addressId]);
  }

  navigateToAddAddress(): void {
    this.router.navigate(['/customer/addresses/new']);
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm.set(target.value);
  }

  getStatusSeverity(
    status: AddressStatus
  ): 'info' | 'warn' | 'success' | 'secondary' | 'danger' | 'contrast' {
    switch (status) {
      case AddressStatus.BinRequested:
        return 'info';
      case AddressStatus.PendingBinPayment:
        return 'warn';
      case AddressStatus.PendingBinDelivery:
        return 'info';
      case AddressStatus.BinDelivered:
        return 'success';
      default:
        return 'secondary';
    }
  }

  getStatusLabel(status: AddressStatus): string {
    switch (status) {
      case AddressStatus.BinRequested:
        return 'Bin Requested';
      case AddressStatus.PendingBinPayment:
        return 'Pending Payment';
      case AddressStatus.PendingBinDelivery:
        return 'Pending Delivery';
      case AddressStatus.BinDelivered:
        return 'Delivered';
      case AddressStatus.PendingAddressCompletion:
        return 'Pending';
      default:
        return 'Unknown';
    }
  }
}
