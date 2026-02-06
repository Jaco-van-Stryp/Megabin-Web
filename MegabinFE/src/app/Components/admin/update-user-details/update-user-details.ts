import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { AutoCompleteCompleteEvent, AutoCompleteModule } from 'primeng/autocomplete';
import {
  AdminService,
  AddressSuggestion,
  CreateDriverCommand,
  GetDriver,
  GetUser,
  UpdateDriverCommand,
  UpdateUserCommand,
  UserRoles,
} from '../../../services';
import { MessageService } from 'primeng/api';
import { InputNumberModule } from 'primeng/inputnumber';
import { Autocomplete } from '../../autocomplete/autocomplete';

@Component({
  selector: 'app-update-user-details',
  imports: [
    DialogModule,
    ButtonModule,
    InputTextModule,
    FormsModule,
    AutoCompleteModule,
    InputNumberModule,
    Autocomplete,
  ],
  templateUrl: './update-user-details.html',
})
export class UpdateUserDetails {
  adminService = inject(AdminService);
  messageService = inject(MessageService);
  visible = signal<boolean>(false);
  User = input.required<GetUser>();

  // Driver state
  driverProfile = signal<GetDriver | null>(null);
  previousRole = signal<UserRoles | null>(null);
  currentRole = signal<UserRoles>(UserRoles.Customer);
  isDriver = computed(() => this.currentRole() === UserRoles.Driver);

  allRoles = [
    { label: 'Admin', value: UserRoles.Admin },
    { label: 'Driver', value: UserRoles.Driver },
    { label: 'Customer', value: UserRoles.Customer },
  ];

  filteredRoles = signal(this.allRoles);

  showDialog() {
    this.visible.set(true);
    this.currentRole.set(this.User().role);
    this.previousRole.set(this.User().role);
    // Load driver profile if user is a driver
    if (this.isDriver()) {
      this.fetchDriverProfile();
    }
  }

  onRoleChange(newRole: UserRoles) {
    // Update the current role signal
    this.currentRole.set(newRole);
    // Also update the User object for saving
    this.User().role = newRole;

    // If user role is Driver, fetch driver profile
    if (newRole === UserRoles.Driver) {
      this.fetchDriverProfile();
    }
    // Note: disableDriver() is now called in updateUserDetails() after successful save
  }

  fetchDriverProfile() {
    this.adminService.apiAdminGetDriverGet(this.User().id).subscribe({
      next: (driver: GetDriver) => {
        this.driverProfile.set(driver);
      },
      error: (error) => {
        // 404 means no driver profile exists yet
        if (error.status === 404) {
          this.driverProfile.set(this.getEmptyDriverProfile());
        } else {
          console.error('Error fetching driver profile:', error);
        }
      },
    });
  }

  getEmptyDriverProfile(): GetDriver {
    return {
      userId: this.User().id,
      homeAddressLabel: null,
      homeAddressLong: 0,
      homeAddressLat: 0,
      dropoffLocationLabel: null,
      dropoffLocationLong: 0,
      dropoffLocationLat: 0,
      vehicleCapacity: 10,
      licenseNumber: null,
      active: true,
      timeZoneId: 'Africa/Johannesburg',
    };
  }

  disableDriver(onSuccess?: () => void) {
    this.adminService.apiAdminDisableDriverPost(this.User().id).subscribe({
      next: () => {
        this.driverProfile.set(null);
        if (onSuccess) {
          onSuccess();
        }
      },
      error: (error) => {
        console.error('Error disabling driver:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to disable driver profile',
        });
      },
    });
  }

  setHomeAddress(addressSuggestion: AddressSuggestion) {
    const driver = this.driverProfile();
    if (!driver) return;

    this.driverProfile.set({
      ...driver,
      homeAddressLabel: addressSuggestion.label || null,
      homeAddressLat: addressSuggestion.location?.latitude || 0,
      homeAddressLong: addressSuggestion.location?.longitude || 0,
    });
  }

  setDropoffLocation(addressSuggestion: AddressSuggestion) {
    const driver = this.driverProfile();
    if (!driver) return;

    this.driverProfile.set({
      ...driver,
      dropoffLocationLabel: addressSuggestion.label || null,
      dropoffLocationLat: addressSuggestion.location?.latitude || 0,
      dropoffLocationLong: addressSuggestion.location?.longitude || 0,
    });
  }

  filterRoles(event: AutoCompleteCompleteEvent) {
    const query = event.query.toLowerCase();
    this.filteredRoles.set(
      this.allRoles.filter((role) => role.label.toLowerCase().includes(query)),
    );
  }

  updateUserDetails() {
    const updateRequest: UpdateUserCommand = {
      userId: this.User().id,
      name: this.User().name,
      email: this.User().email,
      phoneNumber: this.User().phoneNumber,
      role: this.User().role,
    };

    this.adminService.apiAdminUpdateUserPost(updateRequest).subscribe({
      next: () => {
        // Check if role changed FROM Driver to non-Driver
        if (this.previousRole() === UserRoles.Driver && !this.isDriver()) {
          // Disable driver profile after successful user update
          this.disableDriver(() => {
            this.visible.set(false);
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'User updated and driver profile disabled successfully',
            });
            this.previousRole.set(this.currentRole());
          });
        } else if (this.isDriver() && this.driverProfile()) {
          // If user is a driver, handle driver profile update/create
          this.saveDriverProfile();
        } else {
          // Just close and show success
          this.visible.set(false);
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'User details updated successfully',
          });
          this.previousRole.set(this.currentRole());
        }
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to update user details',
        });
        console.error('Error updating user details:', error);
      },
    });
  }

  saveDriverProfile() {
    const driver = this.driverProfile();
    if (!driver) return;

    // If driver has an ID, it exists - call update
    // Otherwise, it's new - call create
    if (driver.driverId) {
      const updateRequest: UpdateDriverCommand = {
        userId: driver.userId,
        homeAddressLabel: driver.homeAddressLabel,
        homeAddressLong: driver.homeAddressLong,
        homeAddressLat: driver.homeAddressLat,
        dropoffLocationLabel: driver.dropoffLocationLabel,
        dropoffLocationLong: driver.dropoffLocationLong,
        dropoffLocationLat: driver.dropoffLocationLat,
        vehicleCapacity: driver.vehicleCapacity,
        licenseNumber: driver.licenseNumber,
        active: driver.active,
        timeZoneId: driver.timeZoneId,
      };

      this.adminService.apiAdminUpdateDriverPost(updateRequest).subscribe({
        next: () => {
          this.visible.set(false);
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'User and driver details updated successfully',
          });
          this.previousRole.set(this.currentRole());
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to update driver details',
          });
          console.error('Error updating driver:', error);
        },
      });
    } else {
      const createRequest: CreateDriverCommand = {
        userId: driver.userId,
        homeAddressLabel: driver.homeAddressLabel,
        homeAddressLong: driver.homeAddressLong,
        homeAddressLat: driver.homeAddressLat,
        dropoffLocationLabel: driver.dropoffLocationLabel,
        dropoffLocationLong: driver.dropoffLocationLong,
        dropoffLocationLat: driver.dropoffLocationLat,
        vehicleCapacity: driver.vehicleCapacity,
        licenseNumber: driver.licenseNumber,
        active: driver.active,
        timeZoneId: driver.timeZoneId,
      };

      this.adminService.apiAdminCreateDriverPost(createRequest).subscribe({
        next: () => {
          this.visible.set(false);
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'User and driver details created successfully',
          });
          this.previousRole.set(this.currentRole());
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to create driver details',
          });
          console.error('Error creating driver:', error);
        },
      });
    }
  }
}
