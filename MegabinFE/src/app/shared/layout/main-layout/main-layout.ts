import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DrawerModule } from 'primeng/drawer';
import { ButtonModule } from 'primeng/button';
import { SidebarComponent } from '../sidebar/sidebar';
import { ResponsiveService } from '../../services/responsive.service';

@Component({
  selector: 'app-main-layout',
  imports: [
    RouterOutlet,
    DrawerModule,
    ButtonModule,
    SidebarComponent,
  ],
  templateUrl: './main-layout.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    class: 'flex min-h-screen w-full',
  },
})
export class MainLayout {
  private responsiveService = inject(ResponsiveService);

  /** Whether the desktop sidebar is collapsed */
  sidebarCollapsed = signal(false);

  /** Whether the mobile drawer is visible */
  sidebarVisible = signal(false);

  /** Reactive signal for mobile detection */
  readonly isMobile = this.responsiveService.isMobile;

  /** Toggle sidebar state based on current device */
  toggleSidebar(): void {
    if (this.isMobile()) {
      this.sidebarVisible.update((v) => !v);
    } else {
      this.sidebarCollapsed.update((v) => !v);
    }
  }

  /** Close the mobile drawer */
  closeMobileSidebar(): void {
    this.sidebarVisible.set(false);
  }
}
