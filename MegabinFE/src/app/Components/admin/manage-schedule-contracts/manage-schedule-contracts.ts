import { Component, input } from '@angular/core';
import { ScheduleContracts } from '../schedule-contracts/schedule-contracts';

@Component({
  selector: 'app-manage-schedule-contracts',
  imports: [ScheduleContracts],
  templateUrl: './manage-schedule-contracts.html',
})
export class ManageScheduleContracts {
  addressId = input.required<string>();
}
