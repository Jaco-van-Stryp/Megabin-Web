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
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { AdminService } from '../../../services/api/admin.service';
import { RouteOptimizationService } from '../../../services/api/routeOptimization.service';
import { ResponsiveService } from '../../../shared/services/responsive.service';
import { GetUser } from '../../../services/model/getUser';
import { GetAddress } from '../../../services/model/getAddress';
import { AddressStatus } from '../../../services/model/addressStatus';
import { Frequency } from '../../../services/model/frequency';
import { DayOfWeek } from '../../../services/model/dayOfWeek';
import { RoutePreviewDto } from '../../../services/model/routePreviewDto';

export interface PendingAddress {
  addressId: string;
  address: string;
  userName: string;
  userEmail: string;
  userId: string;
  totalBins: number;
  currentStatus: AddressStatus;
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
  approvedExternally: boolean;
}

export interface StatusOption {
  label: string;
  value: AddressStatus;
}

export interface ApprovalStatusOption {
  label: string;
  value: boolean;
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
    SelectModule,
    DatePickerModule,
    FormsModule,
  ],
  providers: [MessageService],
  templateUrl: './approval-dashboard.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApprovalDashboard implements OnInit {
  private adminService = inject(AdminService);
  private routeOptimizationService = inject(RouteOptimizationService);
  private messageService = inject(MessageService);
  private responsiveService = inject(ResponsiveService);

  pendingAddresses = signal<PendingAddress[]>([]);
  pendingContracts = signal<PendingScheduleContract[]>([]);
  isLoading = signal(true);
  processingIds = signal<Set<string>>(new Set());

  // Route generation state
  routePreview = signal<RoutePreviewDto | null>(null);
  routePreviewLoading = signal(false);
  routeGenerating = signal(false);
  selectedDate = signal<Date>(new Date());

  readonly isMobile = this.responsiveService.isMobile;

  readonly addressCount = computed(() => this.pendingAddresses().length);
  readonly contractCount = computed(() => this.pendingContracts().length);
  readonly routePreviewCount = computed(() => this.routePreview()?.scheduleContracts?.length ?? 0);
  readonly totalPendingCount = computed(() => this.addressCount() + this.contractCount());

  readonly addressStatusOptions: StatusOption[] = [
    { label: 'Pending Address Completion', value: AddressStatus.PendingAddressCompletion },
    { label: 'Bin Requested', value: AddressStatus.BinRequested },
    { label: 'Pending Bin Payment', value: AddressStatus.PendingBinPayment },
    { label: 'Pending Bin Delivery', value: AddressStatus.PendingBinDelivery },
    { label: 'Bin Delivered', value: AddressStatus.BinDelivered },
  ];

  readonly approvalStatusOptions: ApprovalStatusOption[] = [
    { label: 'Pending Approval', value: false },
    { label: 'Approved', value: true },
  ];

  ngOnInit(): void {
    this.loadPendingItems();
  }

  loadPendingItems(): void {
    this.isLoading.set(true);

    this.adminService.apiAdminGetAllUsersGet().subscribe({
      next: (users) => {
        const allUsers = users;
        if (allUsers.length === 0) {
          this.isLoading.set(false);
          return;
        }

        const addressRequests = allUsers.map((user) =>
          this.adminService.apiAdminGetAllUserAddressesUserIdGet(user.id),
        );

        forkJoin(addressRequests).subscribe({
          next: (addressResults) => {
            const pendingAddressesList: PendingAddress[] = [];
            const addressesForContracts: {
              address: GetAddress;
              user: GetUser;
            }[] = [];

            addressResults.forEach((addresses, index) => {
              const user = allUsers[index];

              addresses.forEach((addr) => {
                // Show all addresses that are NOT in final status (BinDelivered)
                if (addr.addressStatus !== AddressStatus.BinDelivered) {
                  pendingAddressesList.push({
                    addressId: addr.id,
                    address: addr.address || 'Unknown address',
                    userName: user.name || 'Unknown',
                    userEmail: user.email || '',
                    userId: user.id,
                    totalBins: addr.totalBins,
                    currentStatus: addr.addressStatus,
                  });
                }

                // Check for schedule contracts on all addresses (contracts can exist at any stage)
                addressesForContracts.push({ address: addr, user });
              });
            });

            this.pendingAddresses.set(pendingAddressesList);

            if (addressesForContracts.length === 0) {
              this.pendingContracts.set([]);
              this.isLoading.set(false);
              return;
            }

            const contractRequests = addressesForContracts.map((item) =>
              this.adminService.apiAdminGetScheduledContractsAddressIdGet(item.address.id),
            );

            forkJoin(contractRequests).subscribe({
              next: (contractResults) => {
                const pendingContractsList: PendingScheduleContract[] = [];

                contractResults.forEach((contracts, index) => {
                  const { address, user } = addressesForContracts[index];

                  // Show contracts that are NOT approved (final status)
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
                        approvedExternally: contract.approvedExternally,
                      });
                    });
                });

                this.pendingContracts.set(pendingContractsList);
                this.isLoading.set(false);
              },
              error: () => {
                this.pendingContracts.set([]);
                this.isLoading.set(false);
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

  onAddressStatusChange(item: PendingAddress, newStatus: AddressStatus): void {
    this.processingIds.update((ids) => new Set(ids).add(item.addressId));

    this.adminService
      .apiAdminUpdateUserAddressPost({
        addressId: item.addressId,
        status: newStatus,
      })
      .subscribe({
        next: () => {
          // If moved to final status, remove from list
          if (newStatus === AddressStatus.BinDelivered) {
            this.pendingAddresses.update((items) =>
              items.filter((i) => i.addressId !== item.addressId),
            );
          } else {
            // Update the status in the list
            this.pendingAddresses.update((items) =>
              items.map((i) =>
                i.addressId === item.addressId ? { ...i, currentStatus: newStatus } : i,
              ),
            );
          }

          this.processingIds.update((ids) => {
            const newIds = new Set(ids);
            newIds.delete(item.addressId);
            return newIds;
          });

          this.messageService.add({
            severity: 'success',
            summary: 'Updated',
            detail: `Status updated for ${item.address}`,
          });
        },
        error: () => {
          this.processingIds.update((ids) => {
            const newIds = new Set(ids);
            newIds.delete(item.addressId);
            return newIds;
          });
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to update address status',
          });
        },
      });
  }

  onContractApprovalChange(contract: PendingScheduleContract, approved: boolean): void {
    this.processingIds.update((ids) => new Set(ids).add(contract.contractId));

    this.adminService
      .apiAdminUpdateScheduleContractPost({
        contractId: contract.contractId,
        approvedExternally: approved,
        frequency: contract.frequency,
        dayOfWeek: contract.dayOfWeek,
      })
      .subscribe({
        next: () => {
          // If approved (final status), remove from list
          if (approved) {
            this.pendingContracts.update((items) =>
              items.filter((i) => i.contractId !== contract.contractId),
            );
          } else {
            this.pendingContracts.update((items) =>
              items.map((i) =>
                i.contractId === contract.contractId ? { ...i, approvedExternally: approved } : i,
              ),
            );
          }

          this.processingIds.update((ids) => {
            const newIds = new Set(ids);
            newIds.delete(contract.contractId);
            return newIds;
          });

          this.messageService.add({
            severity: 'success',
            summary: 'Updated',
            detail: `Contract ${approved ? 'approved' : 'updated'} for ${contract.address}`,
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
            detail: 'Failed to update contract status',
          });
        },
      });
  }

  isProcessing(id: string): boolean {
    return this.processingIds().has(id);
  }

  formatStatus(status: AddressStatus): string {
    const labels: Record<AddressStatus, string> = {
      pendingAddressCompletion: 'Pending Completion',
      binRequested: 'Bin Requested',
      pendingBinPayment: 'Pending Payment',
      pendingBinDelivery: 'Pending Delivery',
      binDelivered: 'Delivered',
    };
    return labels[status] || status;
  }

  getStatusSeverity(status: AddressStatus): 'secondary' | 'info' | 'warn' | 'success' | 'danger' {
    const severities: Record<AddressStatus, 'secondary' | 'info' | 'warn' | 'success' | 'danger'> =
      {
        pendingAddressCompletion: 'secondary',
        binRequested: 'info',
        pendingBinPayment: 'warn',
        pendingBinDelivery: 'warn',
        binDelivered: 'success',
      };
    return severities[status] || 'secondary';
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

  onDateChange(date: Date): void {
    this.selectedDate.set(date);
    this.loadRoutePreview();
  }

  loadRoutePreview(): void {
    this.routePreviewLoading.set(true);
    const dateStr = this.formatDateForApi(this.selectedDate());

    this.routeOptimizationService.apiRouteOptimizationPreviewDailyRoutesGet(dateStr).subscribe({
      next: (preview) => {
        this.routePreview.set(preview);
        this.routePreviewLoading.set(false);
      },
      error: () => {
        this.routePreviewLoading.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load route preview',
        });
      },
    });
  }

  triggerRouteGeneration(): void {
    this.routeGenerating.set(true);
    const dateStr = this.formatDateForApi(this.selectedDate());

    this.routeOptimizationService.apiRouteOptimizationOptimizeDailyRoutesPost(dateStr).subscribe({
      next: (result) => {
        this.routeGenerating.set(false);
        const routes = result.routes ?? [];
        const totalStops = routes.reduce(
          (sum, route) => sum + (route.stops ?? []).filter((s) => s.type === 'collection').length,
          0,
        );
        this.messageService.add({
          severity: 'success',
          summary: 'Routes Generated',
          detail: `Created ${routes.length} routes with ${totalStops} collection stops`,
        });
        // Reload preview to show updated existing collections count
        this.loadRoutePreview();
      },
      error: (error) => {
        this.routeGenerating.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: error.error || 'Failed to generate routes',
        });
      },
    });
  }

  private formatDateForApi(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}
