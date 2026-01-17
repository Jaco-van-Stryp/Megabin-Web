import { Injectable, signal, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AdminService } from './api/admin.service';
import { AddressService } from './api/address.service';
import { GetUser } from './model/getUser';
import { GetAddress } from './model/getAddress';
import { UpdateUser } from './model/updateUser';
import { ResetPassword } from './model/resetPassword';
import { CreateAddress } from './model/createAddress';
import { CreateAddressResponse } from './model/createAddressResponse';
import { UpdateAddress } from './model/updateAddress';
import { CreateScheduleContract } from './model/createScheduleContract';
import { UpdateScheduleContract } from './model/updateScheduleContract';
import { AddressStatus } from './model/addressStatus';
import { Frequency } from './model/frequency';
import { DayOfWeek } from './model/dayOfWeek';
import { MessageService } from 'primeng/api';

export interface ScheduleContract {
  id: string;
  frequency: Frequency;
  dayOfWeek: DayOfWeek;
  startingDate: string;
  lastCollected: string | null;
  active: boolean;
  approvedExternally: boolean;
  addressesId: string;
}

export interface BreadcrumbItem {
  label: string;
  route: string;
}

@Injectable({
  providedIn: 'root',
})
export class AdminStateService {
  private adminService = inject(AdminService);
  private addressService = inject(AddressService);
  private messageService = inject(MessageService);
  private router = inject(Router);

  // User state
  users = signal<GetUser[]>([]);
  selectedUser = signal<GetUser | null>(null);
  isLoadingUsers = signal(false);

  // Address state
  userAddresses = signal<GetAddress[]>([]);
  selectedAddress = signal<GetAddress | null>(null);
  isLoadingAddresses = signal(false);

  // Schedule state
  addressSchedules = signal<ScheduleContract[]>([]);
  isLoadingSchedules = signal(false);

  // Computed breadcrumbs
  breadcrumbs = computed<BreadcrumbItem[]>(() => {
    const crumbs: BreadcrumbItem[] = [{ label: 'Users', route: '/admin/users' }];

    const user = this.selectedUser();
    if (user && user.name) {
      crumbs.push({
        label: user.name,
        route: `/admin/users/${user.id}`,
      });
    }

    const address = this.selectedAddress();
    if (address && address.address && user) {
      crumbs.push({
        label: address.address.substring(0, 30) + (address.address.length > 30 ? '...' : ''),
        route: `/admin/users/${user.id}/addresses/${address.id}`,
      });
    }

    return crumbs;
  });

  // Load all users
  async loadUsers(): Promise<void> {
    this.isLoadingUsers.set(true);
    try {
      const users = await firstValueFrom(this.adminService.apiAdminGetAllUsersGet());
      this.users.set(users);
    } catch (error) {
      console.error('Failed to load users:', error);
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to load users',
      });
      this.users.set([]);
    } finally {
      this.isLoadingUsers.set(false);
    }
  }

  // Load addresses for a specific user
  async loadUserAddresses(userId: string): Promise<void> {
    this.isLoadingAddresses.set(true);
    try {
      // Get all addresses and filter by userId
      this.adminService.apiAdminGetAllUserAddressesUserIdGet(userId).subscribe({
        next: (addresses) => {
          this.userAddresses.set(addresses as GetAddress[]);
          // Set selected user
        },
        error: (error) => {
          console.error('Failed to load addresses:', error);
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load addresses',
          });
          this.userAddresses.set([]);
        },
        complete: () => {
          this.isLoadingAddresses.set(false);
        },
      });

      // Set selected user
      const user = this.users().find((u) => u.id === userId);
      if (user) {
        this.selectedUser.set(user);
      }
    } catch (error) {
      console.error('Failed to load addresses:', error);
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to load addresses',
      });
      this.userAddresses.set([]);
    } finally {
      this.isLoadingAddresses.set(false);
    }
  }

  // Load schedules for a specific address
  async loadAddressSchedules(addressId: string): Promise<void> {
    this.isLoadingSchedules.set(true);
    try {
      const schedules = await firstValueFrom(
        this.adminService.apiAdminGetScheduledContractsAddressIdGet(addressId),
      );
      this.addressSchedules.set(schedules as ScheduleContract[]);

      // Set selected address
      const address = this.userAddresses().find((a) => a.id === addressId);
      if (address) {
        this.selectedAddress.set(address);
      }
    } catch (error) {
      console.error('Failed to load schedules:', error);
      // Don't show error for 404 (no schedules found)
      if ((error as any)?.status !== 404) {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load schedules',
        });
      }
      this.addressSchedules.set([]);
    } finally {
      this.isLoadingSchedules.set(false);
    }
  }

  // Update user
  async updateUser(updateUser: UpdateUser): Promise<boolean> {
    try {
      await firstValueFrom(this.adminService.apiAdminUpdateUserPost(updateUser));

      // Update local state
      this.users.update((users) =>
        users.map((u) =>
          u.id === updateUser.userId
            ? {
                ...u,
                name: updateUser.name,
                email: updateUser.email,
                role: updateUser.role,
              }
            : u,
        ),
      );

      // Update selected user if it's the one being edited
      if (this.selectedUser()?.id === updateUser.userId) {
        this.selectedUser.update((user) =>
          user
            ? {
                ...user,
                name: updateUser.name,
                email: updateUser.email,
                role: updateUser.role,
              }
            : null,
        );
      }

      this.messageService.add({
        severity: 'success',
        summary: 'Success',
        detail: 'User updated successfully',
      });
      return true;
    } catch (error) {
      console.error('Failed to update user:', error);
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to update user',
      });
      return false;
    }
  }

  // Reset user password
  async resetPassword(userId: string, newPassword: string): Promise<boolean> {
    try {
      const resetPassword: ResetPassword = { userId, newPassword };
      await firstValueFrom(this.adminService.apiAdminResetUserPasswordPost(resetPassword));

      this.messageService.add({
        severity: 'success',
        summary: 'Success',
        detail: 'Password reset successfully',
      });
      return true;
    } catch (error) {
      console.error('Failed to reset password:', error);
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to reset password',
      });
      return false;
    }
  }

  // Delete user
  async deleteUser(userId: string): Promise<boolean> {
    const originalUsers = this.users();

    // Optimistically remove from UI
    this.users.update((users) => users.filter((u) => u.id !== userId));

    try {
      await firstValueFrom(this.adminService.apiAdminDeleteUserUserIdDelete(userId));

      this.messageService.add({
        severity: 'success',
        summary: 'Success',
        detail: 'User deleted successfully',
      });
      return true;
    } catch (error) {
      console.error('Failed to delete user:', error);
      // Rollback on error
      this.users.set(originalUsers);
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to delete user',
      });
      return false;
    }
  }

  // Add address to user
  async addAddress(createAddress: CreateAddress): Promise<string | null> {
    try {
      const response = await firstValueFrom(
        this.adminService.apiAdminAddUserAddressPost(createAddress),
      );

      // Reload addresses for current user
      await this.loadUserAddresses(createAddress.userId);

      this.messageService.add({
        severity: 'success',
        summary: 'Success',
        detail: 'Address added successfully',
      });
      return (response as CreateAddressResponse).addressId || null;
    } catch (error) {
      console.error('Failed to add address:', error);
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to add address',
      });
      return null;
    }
  }

  // Update address
  async updateAddress(updateAddress: UpdateAddress): Promise<boolean> {
    try {
      await firstValueFrom(this.adminService.apiAdminUpdateUserAddressPost(updateAddress));

      // Update local state
      this.userAddresses.update((addresses) =>
        addresses.map((a) =>
          a.id === updateAddress.addressId
            ? {
                ...a,
                totalBins: updateAddress.totalBins,
                addressNotes: updateAddress.addressNotes ?? null,
                addressStatus: updateAddress.status,
              }
            : a,
        ),
      );

      // Update selected address if it's the one being edited
      if (this.selectedAddress()?.id === updateAddress.addressId) {
        this.selectedAddress.update((addr) =>
          addr
            ? {
                ...addr,
                totalBins: updateAddress.totalBins,
                addressNotes: updateAddress.addressNotes ?? null,
                addressStatus: updateAddress.status,
              }
            : null,
        );
      }

      this.messageService.add({
        severity: 'success',
        summary: 'Success',
        detail: 'Address updated successfully',
      });
      return true;
    } catch (error) {
      console.error('Failed to update address:', error);
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to update address',
      });
      return false;
    }
  }

  // Update address status
  async updateAddressStatus(addressId: string, status: AddressStatus): Promise<boolean> {
    const address = this.userAddresses().find((a) => a.id === addressId);
    if (!address) return false;

    const updateAddress: UpdateAddress = {
      addressId,
      totalBins: address.totalBins,
      addressNotes: address.addressNotes || '',
      status,
    };

    return this.updateAddress(updateAddress);
  }

  // Add schedule contract
  async addSchedule(createSchedule: CreateScheduleContract): Promise<boolean> {
    try {
      await firstValueFrom(this.adminService.apiAdminAddScheduleContractPost(createSchedule));

      // Reload schedules for current address
      await this.loadAddressSchedules(createSchedule.addressId);

      this.messageService.add({
        severity: 'success',
        summary: 'Success',
        detail: 'Schedule added successfully',
      });
      return true;
    } catch (error) {
      console.error('Failed to add schedule:', error);
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to add schedule',
      });
      return false;
    }
  }

  // Update schedule contract
  async updateSchedule(updateSchedule: UpdateScheduleContract): Promise<boolean> {
    try {
      await firstValueFrom(this.adminService.apiAdminUpdateScheduleContractPost(updateSchedule));

      // Update local state
      this.addressSchedules.update((schedules) =>
        schedules.map((s) =>
          s.id === updateSchedule.contractId
            ? {
                ...s,
                frequency: updateSchedule.frequency ?? s.frequency,
                dayOfWeek: updateSchedule.dayOfWeek ?? s.dayOfWeek,
                active: updateSchedule.active ?? s.active,
                approvedExternally: updateSchedule.approvedExternally ?? s.approvedExternally,
              }
            : s,
        ),
      );

      this.messageService.add({
        severity: 'success',
        summary: 'Success',
        detail: 'Schedule updated successfully',
      });
      return true;
    } catch (error) {
      console.error('Failed to update schedule:', error);
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Failed to update schedule',
      });
      return false;
    }
  }

  // Delete schedule (sets active to false rather than deleting)
  async deleteSchedule(scheduleId: string): Promise<boolean> {
    const schedule = this.addressSchedules().find((s) => s.id === scheduleId);
    if (!schedule) return false;

    const updateSchedule: UpdateScheduleContract = {
      contractId: scheduleId,
      frequency: schedule.frequency,
      dayOfWeek: schedule.dayOfWeek,
      active: false,
      approvedExternally: schedule.approvedExternally,
    };

    return this.updateSchedule(updateSchedule);
  }

  // Clear selections when navigating away
  clearSelections(): void {
    this.selectedUser.set(null);
    this.selectedAddress.set(null);
    this.addressSchedules.set([]);
  }
}
