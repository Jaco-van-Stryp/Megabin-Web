import { Component, ChangeDetectionStrategy, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import {
  AddressService,
  CustomerService,
  GetAddress,
  CustomerScheduleContractDto,
  DayOfWeek,
  Frequency,
  AddressStatus,
} from '../../../services';

@Component({
  selector: 'app-address-detail',
  imports: [
    CardModule,
    ButtonModule,
    TagModule,
    TableModule,
    DialogModule,
    SelectModule,
    FormsModule,
    ToastModule,
  ],
  providers: [MessageService],
  templateUrl: './address-detail.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AddressDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private addressService = inject(AddressService);
  private customerService = inject(CustomerService);
  private messageService = inject(MessageService);

  addressId = signal<string>('');
  address = signal<GetAddress | null>(null);
  scheduleContracts = signal<CustomerScheduleContractDto[]>([]);
  isLoading = signal<boolean>(true);

  // Schedule request dialog
  showScheduleDialog = signal<boolean>(false);
  selectedFrequency = signal<Frequency | null>(null);
  selectedDayOfWeek = signal<DayOfWeek | null>(null);
  isSubmittingSchedule = signal<boolean>(false);

  frequencyOptions = [
    { label: 'Daily', value: 'Daily' as Frequency },
    { label: 'Weekly', value: 'Weekly' as Frequency },
    { label: 'Bi-Weekly', value: 'BiWeekly' as Frequency },
    { label: 'Monthly', value: 'Monthly' as Frequency },
  ];

  dayOfWeekOptions = [
    { label: 'Monday', value: 'Monday' as DayOfWeek },
    { label: 'Tuesday', value: 'Tuesday' as DayOfWeek },
    { label: 'Wednesday', value: 'Wednesday' as DayOfWeek },
    { label: 'Thursday', value: 'Thursday' as DayOfWeek },
    { label: 'Friday', value: 'Friday' as DayOfWeek },
    { label: 'Saturday', value: 'Saturday' as DayOfWeek },
    { label: 'Sunday', value: 'Sunday' as DayOfWeek },
  ];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('addressId');
    if (id) {
      this.addressId.set(id);
      this.loadData();
    }
  }

  loadData(): void {
    this.isLoading.set(true);

    // Load address
    this.addressService.apiAddressGetAllAddressesGet().subscribe({
      next: (addresses) => {
        const found = addresses.find((a) => a.id === this.addressId());
        this.address.set(found || null);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error loading address:', err);
        this.isLoading.set(false);
      },
    });

    // Load schedule contracts
    this.customerService.apiCustomerGetMyScheduleContractsGet().subscribe({
      next: (contracts) => {
        const filtered = contracts.filter((c) => c.addressId === this.addressId());
        this.scheduleContracts.set(filtered);
      },
      error: (err) => console.error('Error loading schedule contracts:', err),
    });
  }

  openScheduleDialog(): void {
    this.selectedFrequency.set(null);
    this.selectedDayOfWeek.set(null);
    this.showScheduleDialog.set(true);
  }

  submitScheduleRequest(): void {
    const frequency = this.selectedFrequency();
    const dayOfWeek = this.selectedDayOfWeek();

    if (!frequency || !dayOfWeek) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Please select frequency and day of week',
      });
      return;
    }

    this.isSubmittingSchedule.set(true);

    this.customerService
      .apiCustomerRequestScheduleContractPost({
        addressId: this.addressId(),
        frequency: frequency,
        dayOfWeek: dayOfWeek,
      })
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Schedule request submitted. Awaiting admin approval.',
          });
          this.showScheduleDialog.set(false);
          this.loadData();
          this.isSubmittingSchedule.set(false);
        },
        error: (err) => {
          console.error('Error requesting schedule:', err);
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to request schedule',
          });
          this.isSubmittingSchedule.set(false);
        },
      });
  }

  goBack(): void {
    this.router.navigate(['/customer']);
  }

  getStatusSeverity(
    status: string,
  ): 'success' | 'secondary' | 'info' | 'warn' | 'danger' | 'contrast' | undefined {
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

  getStatusLabel(status: string): string {
    switch (status) {
      case AddressStatus.BinRequested:
        return 'Bin Requested';
      case AddressStatus.PendingBinPayment:
        return 'Pending Payment';
      case AddressStatus.PendingBinDelivery:
        return 'Pending Delivery';
      case AddressStatus.BinDelivered:
        return 'Delivered';
      default:
        return 'Pending';
    }
  }
}
