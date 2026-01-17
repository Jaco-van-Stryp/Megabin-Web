import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AutoComplete, AutoCompleteSelectEvent } from 'primeng/autocomplete';
import { AddressService } from '../../services/api/address.service';
import { AddressSuggestion } from '../../services/model/addressSuggestion';

@Component({
  selector: 'app-autocomplete',
  imports: [AutoComplete, FormsModule],
  templateUrl: './autocomplete.html',
  styleUrl: './autocomplete.css',
})
export class Autocomplete {
  placeholder = input<string>('Enter address...');
  label = input<string>('Address');
  addressService = inject(AddressService);

  addressSelected = output<AddressSuggestion>();

  selectedAddress = signal<AddressSuggestion | null>(null);
  suggestions = signal<AddressSuggestion[]>([]);

  onSearch(event: { query: string }) {
    const query = event.query.trim();

    if (query.length < 3) {
      this.suggestions.set([]);
      return;
    }

    this.addressService.apiAddressAutoCompleteAddressGet(query).subscribe({
      next: (results) => {
        this.suggestions.set(results);
      },
      error: (error) => {
        console.error('Error fetching address suggestions:', error);
        this.suggestions.set([]);
      },
    });
  }

  onSelect(event: AutoCompleteSelectEvent) {
    const address = event.value as AddressSuggestion;
    this.selectedAddress.set(address);
    this.addressSelected.emit(address);
  }

  onClear() {
    this.selectedAddress.set(null);
    this.suggestions.set([]);
  }
}
