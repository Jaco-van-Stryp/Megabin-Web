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
    FormsModule,
  ],
  templateUrl: './edit-schedule-dialog.html',
  styleUrls: ['./edit-schedule-dialog.css'],
})
export class EditScheduleDialog {
  visible = input<boolean>(false);
  schedule = input<ScheduleContract | null>(null);
  isEditMode = input<boolean>(false);

  saved = output<{
    frequency: Frequency;
    dayOfWeek: DayOfWeek;
    active: boolean;
    approvedExternally: boolean;
  }>();
  cancelled = output<void>();

  frequency = signal<Frequency>(Frequency.Weekly);
  dayOfWeek = signal<DayOfWeek>(DayOfWeek.Monday);
  active = signal<boolean>(true);
  approvedExternally = signal<boolean>(true);

  frequencyOptions = [
    { label: 'Daily', value: Frequency.Daily },
    { label: 'Weekly', value: Frequency.Weekly },
    { label: 'Bi-Weekly', value: Frequency.BiWeekly },
    { label: 'Monthly', value: Frequency.Monthly },
    { label: 'Yearly', value: Frequency.Yearly },
    { label: 'One-Time', value: Frequency.OneTime },
  ];

  dayOfWeekOptions = [
    { label: 'Monday', value: DayOfWeek.Monday },
    { label: 'Tuesday', value: DayOfWeek.Tuesday },
    { label: 'Wednesday', value: DayOfWeek.Wednesday },
    { label: 'Thursday', value: DayOfWeek.Thursday },
    { label: 'Friday', value: DayOfWeek.Friday },
    { label: 'Saturday', value: DayOfWeek.Saturday },
    { label: 'Sunday', value: DayOfWeek.Sunday },
  ];

  constructor() {
    // Update form when schedule input changes (edit mode)
    effect(() => {
      const currentSchedule = this.schedule();
      if (currentSchedule && this.isEditMode()) {
        this.frequency.set(currentSchedule.frequency);
        this.dayOfWeek.set(currentSchedule.dayOfWeek);
        this.active.set(currentSchedule.active);
        this.approvedExternally.set(currentSchedule.approvedExternally);
      } else if (!this.isEditMode()) {
        // Reset to defaults for add mode
        this.frequency.set(Frequency.Weekly);
        this.dayOfWeek.set(DayOfWeek.Monday);
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
      approvedExternally: this.approvedExternally(),
    });
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}
