import { Component, input, output, signal, ChangeDetectionStrategy, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { CheckboxModule } from 'primeng/checkbox';
import { FloatLabelModule } from 'primeng/floatlabel';
import { Frequency } from '../../../../services/model/frequency';
import { DayOfWeek } from '../../../../services/model/dayOfWeek';
import { ScheduleContract } from '../../../../services/admin-state.service';

@Component({
  selector: 'app-edit-schedule-dialog',
  imports: [
    DialogModule,
    ButtonModule,
    SelectModule,
    CheckboxModule,
    FloatLabelModule,
    FormsModule
  ],
  templateUrl: './edit-schedule-dialog.html',
  styleUrls: ['./edit-schedule-dialog.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EditScheduleDialog {
  visible = input<boolean>(false);
  schedule = input<ScheduleContract | null>(null);
  isEditMode = input<boolean>(false);

  saved = output<{ frequency: Frequency; dayOfWeek: DayOfWeek; active: boolean; approvedExternally: boolean }>();
  cancelled = output<void>();

  frequency = signal<Frequency>(Frequency.NUMBER_1);
  dayOfWeek = signal<DayOfWeek>(DayOfWeek.NUMBER_0);
  active = signal<boolean>(true);
  approvedExternally = signal<boolean>(true);

  frequencyOptions = [
    { label: 'Daily', value: Frequency.NUMBER_0 },
    { label: 'Weekly', value: Frequency.NUMBER_1 },
    { label: 'Bi-Weekly', value: Frequency.NUMBER_2 },
    { label: 'Monthly', value: Frequency.NUMBER_3 },
    { label: 'Yearly', value: Frequency.NUMBER_4 },
    { label: 'One-Time', value: Frequency.NUMBER_5 }
  ];

  dayOfWeekOptions = [
    { label: 'Monday', value: DayOfWeek.NUMBER_0 },
    { label: 'Tuesday', value: DayOfWeek.NUMBER_1 },
    { label: 'Wednesday', value: DayOfWeek.NUMBER_2 },
    { label: 'Thursday', value: DayOfWeek.NUMBER_3 },
    { label: 'Friday', value: DayOfWeek.NUMBER_4 },
    { label: 'Saturday', value: DayOfWeek.NUMBER_5 },
    { label: 'Sunday', value: DayOfWeek.NUMBER_6 }
  ];

  constructor() {
    // Update form when schedule input changes (edit mode)
    effect(() => {
      const currentSchedule = this.schedule();
      if (currentSchedule && this.isEditMode()) {
        this.frequency.set(currentSchedule.frequency as Frequency);
        this.dayOfWeek.set(currentSchedule.dayOfWeek as DayOfWeek);
        this.active.set(currentSchedule.active);
        this.approvedExternally.set(currentSchedule.approvedExternally);
      } else if (!this.isEditMode()) {
        // Reset to defaults for add mode
        this.frequency.set(Frequency.NUMBER_1);
        this.dayOfWeek.set(DayOfWeek.NUMBER_0);
        this.active.set(true);
        this.approvedExternally.set(true);
      }
    });
  }

  onSave(): void {
    this.saved.emit({
      frequency: this.frequency(),
      dayOfWeek: this.dayOfWeek(),
      active: this.active(),
      approvedExternally: this.approvedExternally()
    });
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}
