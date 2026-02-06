import { Component, ChangeDetectionStrategy, inject, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { DividerModule } from 'primeng/divider';
import { AvatarModule } from 'primeng/avatar';
import { MenuConfigService } from '../../services/menu-config.service';
import { AuthTokenService } from '../../../services/auth-token.service';

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive, ButtonModule, TooltipModule, DividerModule, AvatarModule],
  templateUrl: './sidebar.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SidebarComponent {
  private menuConfigService = inject(MenuConfigService);
  private authTokenService = inject(AuthTokenService);
  private router = inject(Router);

  /** Whether the sidebar is collapsed (desktop only) */
  collapsed = input<boolean>(false);

  /** Whether this sidebar is rendered inside a mobile drawer */
  isMobileDrawer = input<boolean>(false);

  /** Event emitted when the collapse toggle button is clicked */
  toggleCollapse = output<void>();

  /** Filtered menu items based on user role */
  readonly menuItems = this.menuConfigService.filteredMenuItems;

  /** Current user information */
  readonly user = this.authTokenService.user;

  /** Get user initials for avatar */
  getUserInitial(): string {
    const name = this.user()?.name;
    return name ? name.charAt(0).toUpperCase() : '?';
  }

  /** Handle logout action */
  logout(): void {
    this.authTokenService.logout();
    this.router.navigate(['/login']);
  }
}
