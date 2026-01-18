import { Component, inject, input, signal } from '@angular/core';
import {
  AdminService,
  DayOfWeek,
  Frequency,
  GetScheduleContract,
  UpdateScheduleContract as UpdateScheduleContractDTO,
} from '../../../services';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { AutoCompleteCompleteEvent, AutoCompleteModule } from 'primeng/autocomplete';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-update-schedule-contract',
  imports: [ButtonModule, AutoCompleteModule, DialogModule, FormsModule],
  templateUrl: './update-schedule-contract.html',
})
export class UpdateScheduleContract {
  contract = input.required<GetScheduleContract>();
  adminService = inject(AdminService);
  messageService = inject(MessageService);
  visible = signal(false);

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

  updateScheduleContract() {
    const updateContract: UpdateScheduleContractDTO = {
      contractId: this.contract().id,
      frequency: this.contract().frequency as Frequency,
      dayOfWeek: this.contract().dayOfWeek as DayOfWeek,
      active: this.contract().active,
      approvedExternally: this.contract().approvedExternally,
    };

    this.adminService.apiAdminUpdateScheduleContractPost(updateContract).subscribe({
      next: () => {
        this.visible.set(false);
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Schedule contract updated successfully',
        });
      },
      error: (error) => {
        console.error('Error updating schedule contract:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to update schedule contract',
        });
      },
    });
  }
}
