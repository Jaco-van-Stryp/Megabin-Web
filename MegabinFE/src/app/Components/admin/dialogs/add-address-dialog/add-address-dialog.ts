import { Component, input, output, signal, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { TextareaModule } from 'primeng/textarea';
import { FloatLabelModule } from 'primeng/floatlabel';
import { Autocomplete } from '../../../autocomplete/autocomplete';
import { AddressSuggestion } from '../../../../services/model/addressSuggestion';

@Component({
  selector: 'app-add-address-dialog',
  imports: [
    DialogModule,
    ButtonModule,
    InputNumberModule,
    TextareaModule,
    FloatLabelModule,
    FormsModule,
    Autocomplete,
  ],
  templateUrl: './add-address-dialog.html',
  styleUrls: ['./add-address-dialog.css'],
})
export class AddAddressDialog {
  visible = input<boolean>(false);

  saved = output<{ address: AddressSuggestion; totalBins: number; addressNotes: string }>();
  cancelled = output<void>();

  selectedAddress = signal<AddressSuggestion | null>(null);
  totalBins = signal<number>(1);
  addressNotes = signal<string>('');

  onAddressSelected(address: AddressSuggestion): void {
    this.selectedAddress.set(address);
  }

  onSave(): void {
    const address = this.selectedAddress();
    if (!address) {
      return;
    }

    this.saved.emit({
      address,
      totalBins: this.totalBins(),
      addressNotes: this.addressNotes(),
    });

    // Reset form
    this.selectedAddress.set(null);
    this.totalBins.set(1);
    this.addressNotes.set('');
  }

  onCancel(): void {
    // Reset form
    this.selectedAddress.set(null);
    this.totalBins.set(1);
    this.addressNotes.set('');
    this.cancelled.emit();
  }
}
