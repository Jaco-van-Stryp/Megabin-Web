import { Component, input, output, signal, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { FloatLabelModule } from 'primeng/floatlabel';

@Component({
  selector: 'app-reset-password-dialog',
  imports: [
    DialogModule,
    ButtonModule,
    PasswordModule,
    FloatLabelModule,
    FormsModule
  ],
  templateUrl: './reset-password-dialog.html',
  styleUrls: ['./reset-password-dialog.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ResetPasswordDialog {
  visible = input<boolean>(false);
  userName = input<string>('');

  saved = output<string>();
  cancelled = output<void>();

  newPassword = signal<string>('');

  onSave(): void {
    if (!this.newPassword()) {
      return;
    }

    this.saved.emit(this.newPassword());
    this.newPassword.set('');
  }

  onCancel(): void {
    this.newPassword.set('');
    this.cancelled.emit();
  }
}
