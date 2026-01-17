import { Component, inject, computed, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { ToastModule } from 'primeng/toast';
import { MenuItem } from 'primeng/api';
import { AdminStateService } from '../../../services/admin-state.service';

@Component({
  selector: 'app-admin-dashboard',
  imports: [RouterOutlet, BreadcrumbModule, ToastModule],
  templateUrl: './admin-dashboard.html',
  styleUrls: ['./admin-dashboard.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminDashboard {
  adminState = inject(AdminStateService);

  // Convert breadcrumbs to PrimeNG MenuItem format
  menuItems = computed<MenuItem[]>(() =>
    this.adminState.breadcrumbs().map(item => ({
      label: item.label,
      routerLink: item.route
    }))
  );
}
