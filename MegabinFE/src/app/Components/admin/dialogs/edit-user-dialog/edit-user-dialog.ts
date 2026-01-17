import { Component, input, output, signal, ChangeDetectionStrategy, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { FloatLabelModule } from 'primeng/floatlabel';
import { GetUser } from '../../../../services/model/getUser';
import { UserRoles } from '../../../../services/model/userRoles';

@Component({
  selector: 'app-edit-user-dialog',
  imports: [
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    SelectModule,
    FloatLabelModule,
    FormsModule,
  ],
  templateUrl: './edit-user-dialog.html',
  styleUrls: ['./edit-user-dialog.css'],
})
export class EditUserDialog {
  visible = input<boolean>(false);
  user = input<GetUser | null>(null);

  saved = output<{ name: string; email: string; role: UserRoles; totalBins: number }>();
  cancelled = output<void>();

  name = signal<string>('');
  email = signal<string>('');
  role = signal<UserRoles>(UserRoles.Customer);
  totalBins = signal<number>(0);

  roleOptions = [
    { label: 'Customer', value: UserRoles.Customer },
    { label: 'Driver', value: UserRoles.Driver },
    { label: 'Admin', value: UserRoles.Admin },
  ];

  constructor() {
    // Update form when user input changes
    effect(() => {
      const currentUser = this.user();
      if (currentUser) {
        this.name.set(currentUser.name || '');
        this.email.set(currentUser.email || '');
        this.role.set(currentUser.role);
        // Total bins is not in GetUser, so we'll keep it at 0 for now
        // The backend UpdateUser endpoint accepts it though
        this.totalBins.set(0);
      }
    });
  }

  onSave(): void {
    if (!this.name() || !this.email()) {
      return;
    }

    this.saved.emit({
      name: this.name(),
      email: this.email(),
      role: this.role(),
      totalBins: this.totalBins(),
    });
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}
