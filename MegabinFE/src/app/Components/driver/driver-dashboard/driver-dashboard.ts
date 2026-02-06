import {
  Component,
  ChangeDetectionStrategy,
  inject,
  OnInit,
  signal,
  computed,
} from '@angular/core';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';
import { FormsModule } from '@angular/forms';
import { TooltipModule } from 'primeng/tooltip';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageService } from 'primeng/api';
import { DriverService, ScheduledCollectionDto } from '../../../services';
import { ResponsiveService } from '../../../shared/services/responsive.service';

@Component({
  selector: 'app-driver-dashboard',
  imports: [
    TableModule,
    TagModule,
    ButtonModule,
    CardModule,
    DialogModule,
    TextareaModule,
    FormsModule,
    TooltipModule,
    ProgressSpinnerModule,
  ],
  providers: [MessageService],
  templateUrl: './driver-dashboard.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DriverDashboard implements OnInit {
  private driverService = inject(DriverService);
  private messageService = inject(MessageService);
  private responsiveService = inject(ResponsiveService);

  collections = signal<ScheduledCollectionDto[]>([]);
  isLoading = signal<boolean>(true);

  // Notes dialog state
  showNotesDialog = signal<boolean>(false);
  selectedCollection = signal<ScheduledCollectionDto | null>(null);
  collectionNotes = signal<string>('');

  readonly isMobile = this.responsiveService.isMobile;

  // Computed values
  completedCount = computed(() => this.collections().filter((c) => c.collected).length);
  totalCount = computed(() => this.collections().length);
  nextStop = computed(() => this.collections().find((c) => !c.collected));
  progressPercentage = computed(() => {
    const total = this.totalCount();
    if (total === 0) return 0;
    return Math.round((this.completedCount() / total) * 100);
  });

  ngOnInit(): void {
    this.loadTodaysRoute();
  }

  loadTodaysRoute(): void {
    this.isLoading.set(true);
    this.driverService.apiDriverTodaysRouteGet().subscribe({
      next: (collections) => {
        this.collections.set(collections);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error loading route:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: "Failed to load today's route",
        });
        this.isLoading.set(false);
      },
    });
  }

  openGoogleMaps(collection: ScheduledCollectionDto): void {
    const url = `https://www.google.com/maps/dir/?api=1&destination=${collection.latitude},${collection.longitude}`;
    window.open(url, '_blank');
  }

  markComplete(collection: ScheduledCollectionDto): void {
    this.driverService
      .apiDriverCollectionCollectionIdPatch(collection.id, {
        collected: true,
        notes: collection.notes ?? undefined,
      })
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Collection marked as complete',
          });
          this.loadTodaysRoute();
        },
        error: (err) => {
          console.error('Error marking collection:', err);
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to update collection status',
          });
        },
      });
  }

  openNotesDialog(collection: ScheduledCollectionDto): void {
    this.selectedCollection.set(collection);
    this.collectionNotes.set(collection.notes ?? '');
    this.showNotesDialog.set(true);
  }

  saveNotes(): void {
    const collection = this.selectedCollection();
    if (!collection) return;

    this.driverService
      .apiDriverCollectionCollectionIdPatch(collection.id, {
        collected: collection.collected,
        notes: this.collectionNotes() || undefined,
      })
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Notes saved',
          });
          this.showNotesDialog.set(false);
          this.loadTodaysRoute();
        },
        error: (err) => {
          console.error('Error saving notes:', err);
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to save notes',
          });
        },
      });
  }

  closeNotesDialog(): void {
    this.showNotesDialog.set(false);
  }
}
