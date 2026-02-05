import {
  Component,
  ChangeDetectionStrategy,
  inject,
  signal,
  computed,
  OnInit,
} from '@angular/core';
import { forkJoin } from 'rxjs';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { Tabs, TabList, Tab, TabPanels, TabPanel } from 'primeng/tabs';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { BadgeModule } from 'primeng/badge';
import { TooltipModule } from 'primeng/tooltip';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { AdminService } from '../../../services/api/admin.service';
import { ResponsiveService } from '../../../shared/services/responsive.service';
import { GetUser } from '../../../services/model/getUser';
import { GetAddress } from '../../../services/model/getAddress';
import { GetScheduleContract } from '../../../services/model/getScheduleContract';
import { AddressStatus } from '../../../services/model/addressStatus';
import { Frequency } from '../../../services/model/frequency';
import { DayOfWeek } from '../../../services/model/dayOfWeek';
import { UserRoles } from '../../../services/model/userRoles';

export interface PendingBinRequest {
  addressId: string;
  address: string;
  userName: string;
  userEmail: string;
  userId: string;
  totalBins: number;
  selected?: boolean;
}

export interface PendingScheduleContract {
  contractId: string;
  addressId: string;
  address: string;
  userName: string;
  userEmail: string;
  userId: string;
  frequency: Frequency;
  dayOfWeek: DayOfWeek;
  selected?: boolean;
}

@Component({
  selector: 'app-approval-dashboard',
  imports: [
    TableModule,
    CardModule,
    Tabs,
    TabList,
    Tab,
    TabPanels,
    TabPanel,
    TagModule,
    ButtonModule,
    BadgeModule,
    TooltipModule,
    ProgressSpinnerModule,
    ConfirmDialogModule,
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './approval-dashboard.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApprovalDashboard implements OnInit {
  private adminService = inject(AdminService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private responsiveService = inject(ResponsiveService);

  pendingBinRequests = signal<PendingBinRequest[]>([]);
  pendingContracts = signal<PendingScheduleContract[]>([]);
  isLoading = signal(true);
  processingIds = signal<Set<string>>(new Set());

  readonly isMobile = this.responsiveService.isMobile;

  readonly binRequestCount = computed(() => this.pendingBinRequests().length);
  readonly contractCount = computed(() => this.pendingContracts().length);
  readonly totalPendingCount = computed(
    () => this.binRequestCount() + this.contractCount()
  );

  readonly selectedBinRequests = computed(() =>
    this.pendingBinRequests().filter((r) => r.selected)
  );
  readonly selectedContracts = computed(() =>
    this.pendingContracts().filter((c) => c.selected)
  );

  ngOnInit(): void {
    this.loadPendingItems();
  }

  loadPendingItems(): void {
    this.isLoading.set(true);

    this.adminService.apiAdminGetAllUsersGet().subscribe({
      next: (users) => {
        const customerUsers = users.filter(
          (u) => u.role === UserRoles.Customer
        );
        if (customerUsers.length === 0) {
          this.isLoading.set(false);
          return;
        }

        const addressRequests = customerUsers.map((user) =>
          this.adminService.apiAdminGetAllUserAddressesUserIdGet(user.id)
        );

        forkJoin(addressRequests).subscribe({
          next: (addressResults) => {
            const binRequests: PendingBinRequest[] = [];
            const addressesWithContracts: {
              address: GetAddress;
              user: GetUser;
            }[] = [];

            addressResults.forEach((addresses, index) => {
              const user = customerUsers[index];

              addresses.forEach((addr) => {
                if (addr.addressStatus === AddressStatus.BinRequested) {
                  binRequests.push({
                    addressId: addr.id,
                    address: addr.address || 'Unknown address',
                    userName: user.name || 'Unknown',
                    userEmail: user.email || '',
                    userId: user.id,
                    totalBins: addr.totalBins,
                  });
                }

                if (addr.addressStatus === AddressStatus.BinDelivered) {
                  addressesWithContracts.push({ address: addr, user });
                }
              });
            });

            this.pendingBinRequests.set(binRequests);

            if (addressesWithContracts.length === 0) {
              this.pendingContracts.set([]);
              this.isLoading.set(false);
              return;
            }

            const contractRequests = addressesWithContracts.map((item) =>
              this.adminService.apiAdminGetScheduledContractsAddressIdGet(
                item.address.id
              )
            );

            forkJoin(contractRequests).subscribe({
              next: (contractResults) => {
                const pendingContractsList: PendingScheduleContract[] = [];

                contractResults.forEach((contracts, index) => {
                  const { address, user } = addressesWithContracts[index];

                  contracts
                    .filter((c) => !c.approvedExternally)
                    .forEach((contract) => {
                      pendingContractsList.push({
                        contractId: contract.id,
                        addressId: address.id,
                        address: address.address || 'Unknown address',
                        userName: user.name || 'Unknown',
                        userEmail: user.email || '',
                        userId: user.id,
                        frequency: contract.frequency,
                        dayOfWeek: contract.dayOfWeek,
                      });
                    });
                });

                this.pendingContracts.set(pendingContractsList);
                this.isLoading.set(false);
              },
              error: () => {
                this.isLoading.set(false);
                this.messageService.add({
                  severity: 'error',
                  summary: 'Error',
                  detail: 'Failed to load schedule contracts',
                });
              },
            });
          },
          error: () => {
            this.isLoading.set(false);
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to load addresses',
            });
          },
        });
      },
      error: () => {
        this.isLoading.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load users',
        });
      },
    });
  }

  toggleBinRequestSelection(request: PendingBinRequest): void {
    this.pendingBinRequests.update((items) =>
      items.map((item) =>
        item.addressId === request.addressId
          ? { ...item, selected: !item.selected }
          : item
      )
    );
  }

  toggleContractSelection(contract: PendingScheduleContract): void {
    this.pendingContracts.update((items) =>
      items.map((item) =>
        item.contractId === contract.contractId
          ? { ...item, selected: !item.selected }
          : item
      )
    );
  }

  selectAllBinRequests(selected: boolean): void {
    this.pendingBinRequests.update((items) =>
      items.map((item) => ({ ...item, selected }))
    );
  }

  selectAllContracts(selected: boolean): void {
    this.pendingContracts.update((items) =>
      items.map((item) => ({ ...item, selected }))
    );
  }

  approveBinRequest(request: PendingBinRequest): void {
    this.processingIds.update((ids) => new Set(ids).add(request.addressId));

    this.adminService
      .apiAdminUpdateUserAddressPost({
        addressId: request.addressId,
        status: AddressStatus.PendingBinPayment,
      })
      .subscribe({
        next: () => {
          this.pendingBinRequests.update((items) =>
            items.filter((item) => item.addressId !== request.addressId)
          );
          this.processingIds.update((ids) => {
            const newIds = new Set(ids);
            newIds.delete(request.addressId);
            return newIds;
          });
          this.messageService.add({
            severity: 'success',
            summary: 'Approved',
            detail: `Bin request approved for ${request.address}`,
          });
        },
        error: () => {
          this.processingIds.update((ids) => {
            const newIds = new Set(ids);
            newIds.delete(request.addressId);
            return newIds;
          });
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to approve bin request',
          });
        },
      });
  }

  bulkApproveBinRequests(): void {
    const selected = this.selectedBinRequests();
    if (selected.length === 0) return;

    this.confirmationService.confirm({
      message: `Approve ${selected.length} bin request(s)?`,
      header: 'Confirm Bulk Approval',
      icon: 'pi pi-check-circle',
      accept: () => {
        selected.forEach((request) => this.approveBinRequest(request));
      },
    });
  }

  approveContract(contract: PendingScheduleContract): void {
    this.processingIds.update((ids) => new Set(ids).add(contract.contractId));

    this.adminService
      .apiAdminUpdateScheduleContractPost({
        contractId: contract.contractId,
        approvedExternally: true,
      })
      .subscribe({
        next: () => {
          this.pendingContracts.update((items) =>
            items.filter((item) => item.contractId !== contract.contractId)
          );
          this.processingIds.update((ids) => {
            const newIds = new Set(ids);
            newIds.delete(contract.contractId);
            return newIds;
          });
          this.messageService.add({
            severity: 'success',
            summary: 'Approved',
            detail: `Schedule contract approved for ${contract.address}`,
          });
        },
        error: () => {
          this.processingIds.update((ids) => {
            const newIds = new Set(ids);
            newIds.delete(contract.contractId);
            return newIds;
          });
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to approve schedule contract',
          });
        },
      });
  }

  bulkApproveContracts(): void {
    const selected = this.selectedContracts();
    if (selected.length === 0) return;

    this.confirmationService.confirm({
      message: `Approve ${selected.length} schedule contract(s)?`,
      header: 'Confirm Bulk Approval',
      icon: 'pi pi-check-circle',
      accept: () => {
        selected.forEach((contract) => this.approveContract(contract));
      },
    });
  }

  isProcessing(id: string): boolean {
    return this.processingIds().has(id);
  }

  formatFrequency(frequency: Frequency): string {
    const labels: Record<Frequency, string> = {
      daily: 'Daily',
      weekly: 'Weekly',
      biWeekly: 'Bi-Weekly',
      monthly: 'Monthly',
      yearly: 'Yearly',
      oneTime: 'One-Time',
    };
    return labels[frequency] || frequency;
  }

  formatDayOfWeek(day: DayOfWeek): string {
    return day.charAt(0).toUpperCase() + day.slice(1);
  }
}
