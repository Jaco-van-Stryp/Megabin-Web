import { Component, inject, input, OnInit, signal } from '@angular/core';
import { AdminService, GetScheduleContract } from '../../../services';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { TooltipModule } from 'primeng/tooltip';
import { UpdateScheduleContract } from '../update-schedule-contract/update-schedule-contract';
import { AddScheduleContract } from '../add-schedule-contract/add-schedule-contract';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-schedule-contracts',
  imports: [
    TableModule,
    TagModule,
    InputTextModule,
    IconFieldModule,
    InputIconModule,
    TooltipModule,
    UpdateScheduleContract,
    AddScheduleContract,
    DatePipe,
  ],
  templateUrl: './schedule-contracts.html',
})
export class ScheduleContracts implements OnInit {
  adminService = inject(AdminService);
  scheduleContracts = signal<GetScheduleContract[]>([]);
  addressId = input.required<string>();

  ngOnInit(): void {
    this.loadScheduleContracts();
  }

  loadScheduleContracts() {
    this.adminService.apiAdminGetScheduledContractsAddressIdGet(this.addressId()).subscribe({
      next: (response: GetScheduleContract[]) => {
        this.scheduleContracts.set(response);
      },
      error: (error) => {
        console.error('Error fetching schedule contracts:', error);
        this.scheduleContracts.set([]);
      },
    });
  }

  clickScheduleContract(contractId: string) {
    console.log('Schedule contract clicked:', contractId);
  }
}
