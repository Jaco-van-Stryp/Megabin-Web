import { Component, inject, input, OnInit, signal } from '@angular/core';
import { AdminService, AddScheduleContractCommand, DayOfWeek, Frequency } from '../../../services';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { AutoCompleteCompleteEvent, AutoCompleteModule } from 'primeng/autocomplete';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-schedule-contract',
  imports: [ButtonModule, AutoCompleteModule, DialogModule, FormsModule],
  templateUrl: './add-schedule-contract.html',
})
export class AddScheduleContract implements OnInit {
  adminService = inject(AdminService);
  addressId = input.required<string>();
  messageService = inject(MessageService);
  scheduleContract = signal<AddScheduleContractCommand>({} as AddScheduleContractCommand);
  visible = signal(false);
  routerService = inject(Router);

  ngOnInit(): void {
    this.scheduleContract().addressId = this.addressId();
  }

  showDialog() {
    this.visible.set(true);
  }

  allFrequencies = [
    { label: 'Daily', value: Frequency.Daily },
    { label: 'Weekly', value: Frequency.Weekly },
    { label: 'Bi-Weekly', value: Frequency.BiWeekly },
    { label: 'Monthly', value: Frequency.Monthly },
    { label: 'Yearly', value: Frequency.Yearly },
    { label: 'One Time', value: Frequency.OneTime },
  ];

  filteredFrequencies = signal(this.allFrequencies);

  filterFrequencies(event: AutoCompleteCompleteEvent) {
    const query = event.query.toLowerCase();
    this.filteredFrequencies.set(
      this.allFrequencies.filter((freq) => freq.label.toLowerCase().includes(query)),
    );
  }

  allDaysOfWeek = [
    { label: 'Monday', value: DayOfWeek.Monday },
    { label: 'Tuesday', value: DayOfWeek.Tuesday },
    { label: 'Wednesday', value: DayOfWeek.Wednesday },
    { label: 'Thursday', value: DayOfWeek.Thursday },
    { label: 'Friday', value: DayOfWeek.Friday },
    { label: 'Saturday', value: DayOfWeek.Saturday },
    { label: 'Sunday', value: DayOfWeek.Sunday },
  ];

  filteredDaysOfWeek = signal(this.allDaysOfWeek);

  filterDaysOfWeek(event: AutoCompleteCompleteEvent) {
    const query = event.query.toLowerCase();
    this.filteredDaysOfWeek.set(
      this.allDaysOfWeek.filter((day) => day.label.toLowerCase().includes(query)),
    );
  }

  createScheduleContract() {
    this.adminService.apiAdminAddScheduleContractPost(this.scheduleContract()).subscribe({
      next: () => {
        this.visible.set(false);
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Schedule contract created successfully',
        });
        this.routerService.navigate(['/admin/manage-schedule-contracts', this.addressId()]);
      },
      error: (error) => {
        console.error('Error creating schedule contract:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to create schedule contract',
        });
      },
    });
  }
}
