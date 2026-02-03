import { Component, ChangeDetectionStrategy, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { AddressService, GetAddress } from '../../../services';

@Component({
  selector: 'app-my-addresses',
  imports: [
    TableModule,
    TagModule,
    ButtonModule,
    InputTextModule,
    IconFieldModule,
    InputIconModule,
  ],
  templateUrl: './my-addresses.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyAddresses implements OnInit {
  private addressService = inject(AddressService);
  private router = inject(Router);

  addresses = signal<GetAddress[]>([]);

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

  addNewAddress(): void {
    this.router.navigate(['/customer/addresses/new']);
  }

  goBack(): void {
    this.router.navigate(['/customer']);
  }
}
