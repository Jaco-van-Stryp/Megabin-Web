import { Component, input } from '@angular/core';
import { UserAddresses } from '../user-addresses/user-addresses';

@Component({
  selector: 'app-manage-user',
  imports: [UserAddresses],
  templateUrl: './manage-user.html',
})
export class ManageUser {
  userId = input.required<string>();
}
