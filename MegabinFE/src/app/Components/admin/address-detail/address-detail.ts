import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { StepperModule } from 'primeng/stepper';
import { DataViewModule } from 'primeng/dataview';
import { SkeletonModule } from 'primeng/skeleton';
import { AdminStateService, ScheduleContract } from '../../../services/admin-state.service';
import { AddressStatus } from '../../../services/model/addressStatus';
import { Frequency } from '../../../services/model/frequency';
import { DayOfWeek } from '../../../services/model/dayOfWeek';
import { EditScheduleDialog } from '../dialogs/edit-schedule-dialog/edit-schedule-dialog';
import { ConfirmDialog } from '../dialogs/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-address-detail',
  imports: [
    CommonModule,
    DatePipe,
    CardModule,
    ButtonModule,
    TagModule,
    StepperModule,
    DataViewModule,
    SkeletonModule,
    EditScheduleDialog,
    ConfirmDialog,
  ],
  templateUrl: './address-detail.html',
  styleUrls: ['./address-detail.css'],
})
export class AddressDetail implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  adminState = inject(AdminStateService);

  userId = signal<string>('');
  addressId = signal<string>('');
  showAddScheduleDialog = signal<boolean>(false);
  showEditScheduleDialog = signal<boolean>(false);
  showDeleteScheduleDialog = signal<boolean>(false);
  showStatusChangeDialog = signal<boolean>(false);
  selectedSchedule = signal<ScheduleContract | null>(null);
  scheduleToDelete = signal<ScheduleContract | null>(null);
  targetStatus = signal<AddressStatus>(AddressStatus.PendingAddressCompletion);

  currentAddress = computed(() => this.adminState.selectedAddress());

  addressStatuses = [
    {
      value: AddressStatus.PendingAddressCompletion,
      label: 'Pending Completion',
      icon: 'pi-clock',
    },
    { value: AddressStatus.BinRequested, label: 'Bin Requested', icon: 'pi-shopping-cart' },
    { value: AddressStatus.PendingBinPayment, label: 'Pending Payment', icon: 'pi-dollar' },
    { value: AddressStatus.PendingBinDelivery, label: 'Pending Delivery', icon: 'pi-truck' },
    { value: AddressStatus.BinDelivered, label: 'Bin Delivered', icon: 'pi-check-circle' },
  ];

  currentStatusIndex = computed(() => {
    const address = this.currentAddress();
    if (!address) return 0;
    return this.addressStatuses.findIndex((s) => s.value === address.addressStatus);
  });

  statusChangeMessage = computed(() => {
    const index = this.addressStatuses.findIndex((s) => s.value === this.targetStatus());
    if (index === -1) return 'Are you sure you want to change the address status?';
    return `Are you sure you want to change the address status to ${this.addressStatuses[index].label}?`;
  });

  ngOnInit(): void {
    const userIdParam = this.route.snapshot.paramMap.get('userId');
    const addressIdParam = this.route.snapshot.paramMap.get('addressId');

    if (userIdParam && addressIdParam) {
      this.userId.set(userIdParam);
      this.addressId.set(addressIdParam);
      this.adminState.loadUserAddresses(userIdParam);
      this.adminState.loadAddressSchedules(addressIdParam);
    } else {
      this.router.navigate(['/admin/users']);
    }
  }

  goBack(): void {
    this.router.navigate(['/admin/users', this.userId()]);
  }

  onStepClick(index: number): void {
    const status = this.addressStatuses[index].value;
    this.targetStatus.set(status);
    this.showStatusChangeDialog.set(true);
  }

  async confirmStatusChange(): Promise<void> {
    const success = await this.adminState.updateAddressStatus(
      this.addressId(),
      this.targetStatus(),
    );

    if (success) {
      this.showStatusChangeDialog.set(false);
    }
  }

  onAddSchedule(): void {
    this.selectedSchedule.set(null);
    this.showAddScheduleDialog.set(true);
  }

  onEditSchedule(schedule: ScheduleContract): void {
    this.selectedSchedule.set(schedule);
    this.showEditScheduleDialog.set(true);
  }

  onDeleteSchedule(schedule: ScheduleContract): void {
    this.scheduleToDelete.set(schedule);
    this.showDeleteScheduleDialog.set(true);
  }

  async onSaveSchedule(data: {
    frequency: Frequency;
    dayOfWeek: DayOfWeek;
    active: boolean;
    approvedExternally: boolean;
  }): Promise<void> {
    const success = await this.adminState.addSchedule({
      addressId: this.addressId(),
      frequency: data.frequency,
      dayOfWeek: data.dayOfWeek,
    });

    if (success) {
      this.showAddScheduleDialog.set(false);
    }
  }

  async onUpdateSchedule(data: {
    frequency: Frequency;
    dayOfWeek: DayOfWeek;
    active: boolean;
    approvedExternally: boolean;
  }): Promise<void> {
    const schedule = this.selectedSchedule();
    if (!schedule) return;

    const success = await this.adminState.updateSchedule({
      contractId: schedule.id,
      frequency: data.frequency,
      dayOfWeek: data.dayOfWeek,
      active: data.active,
      approvedExternally: data.approvedExternally,
    });

    if (success) {
      this.showEditScheduleDialog.set(false);
      this.selectedSchedule.set(null);
    }
  }

  async confirmDelete(): Promise<void> {
    const schedule = this.scheduleToDelete();
    if (!schedule) return;

    const success = await this.adminState.deleteSchedule(schedule.id);

    if (success) {
      this.showDeleteScheduleDialog.set(false);
      this.scheduleToDelete.set(null);
    }
  }

  getFrequencyLabel(frequency: Frequency): string {
    const labels: { [key: string]: string } = {
      [Frequency.Daily]: 'Daily',
      [Frequency.Weekly]: 'Weekly',
      [Frequency.BiWeekly]: 'Bi-Weekly',
      [Frequency.Monthly]: 'Monthly',
      [Frequency.Yearly]: 'Yearly',
      [Frequency.OneTime]: 'One-Time',
    };
    return labels[frequency] || 'Unknown';
  }

  getDayLabel(day: DayOfWeek): string {
    const labels: { [key: string]: string } = {
      [DayOfWeek.Monday]: 'Monday',
      [DayOfWeek.Tuesday]: 'Tuesday',
      [DayOfWeek.Wednesday]: 'Wednesday',
      [DayOfWeek.Thursday]: 'Thursday',
      [DayOfWeek.Friday]: 'Friday',
      [DayOfWeek.Saturday]: 'Saturday',
      [DayOfWeek.Sunday]: 'Sunday',
    };
    return labels[day] || 'Unknown';
  }
}
