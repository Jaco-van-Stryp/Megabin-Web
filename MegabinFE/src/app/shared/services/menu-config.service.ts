import { Injectable, inject, computed } from '@angular/core';
import { AuthTokenService } from '../../services/auth-token.service';
import { UserRoles } from '../../services/model/userRoles';

export interface MenuItem {
  label: string;
  icon: string;
  routerLink: string;
  roles: UserRoles[];
  badge?: number;
}

/**
 * Service for providing role-based menu configuration
 */
@Injectable({
  providedIn: 'root',
})
export class MenuConfigService {
  private authTokenService = inject(AuthTokenService);

  private readonly menuItems: MenuItem[] = [
    // Admin menu items
    {
      label: 'Dashboard',
      icon: 'pi pi-home',
      routerLink: '/admin/dashboard',
      roles: [UserRoles.Admin],
    },
    {
      label: 'Approvals',
      icon: 'pi pi-check-circle',
      routerLink: '/admin/approvals',
      roles: [UserRoles.Admin],
    },
    {
      label: 'Users',
      icon: 'pi pi-users',
      routerLink: '/admin/users',
      roles: [UserRoles.Admin],
    },

    // Driver menu items
    {
      label: "Today's Route",
      icon: 'pi pi-map',
      routerLink: '/driver',
      roles: [UserRoles.Driver],
    },

    // Customer menu items
    {
      label: 'Dashboard',
      icon: 'pi pi-home',
      routerLink: '/customer',
      roles: [UserRoles.Customer],
    },
    {
      label: 'My Addresses',
      icon: 'pi pi-map-marker',
      routerLink: '/customer/addresses',
      roles: [UserRoles.Customer],
    },
    {
      label: 'Profile',
      icon: 'pi pi-user',
      routerLink: '/customer/profile',
      roles: [UserRoles.Customer],
    },
  ];

  /** Computed signal that filters menu items based on the current user's role */
  readonly filteredMenuItems = computed(() => {
    const user = this.authTokenService.user();
    if (!user?.role) return [];
    return this.menuItems.filter((item) =>
      item.roles.includes(user.role as UserRoles)
    );
  });
}
