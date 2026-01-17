import { Component, input, output, ChangeDetectionStrategy } from '@angular/core';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-confirm-dialog',
  imports: [DialogModule, ButtonModule],
  templateUrl: './confirm-dialog.html',
  styleUrls: ['./confirm-dialog.css'],
})
export class ConfirmDialog {
  visible = input<boolean>(false);
  header = input<string>('Confirm');
  message = input<string>('Are you sure?');
  confirmLabel = input<string>('Confirm');
  cancelLabel = input<string>('Cancel');
  severity = input<'success' | 'info' | 'warn' | 'danger'>('warn');

  confirmed = output<void>();
  cancelled = output<void>();

  onConfirm(): void {
    this.confirmed.emit();
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}
