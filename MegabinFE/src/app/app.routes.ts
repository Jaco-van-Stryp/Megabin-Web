import { Routes } from '@angular/router';
import { authGuard, roleGuard } from './shared/guards/auth.guard';
import { UserRoles } from './services/model/userRoles';

export const routes: Routes = [
  // Public routes (no layout)
  {
    path: 'login',
    loadComponent: () => import('./Components/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    loadComponent: () => import('./Components/register/register').then((m) => m.Register),
  },

  // Protected routes (with layout)
  {
    path: '',
    loadComponent: () =>
      import('./shared/layout/main-layout/main-layout').then((m) => m.MainLayout),
    canActivate: [authGuard],
    children: [
      // Admin routes
      {
        path: 'admin',
        canActivate: [roleGuard(UserRoles.Admin)],
        children: [
          {
            path: '',
            redirectTo: 'dashboard',
            pathMatch: 'full',
          },
          {
            path: 'dashboard',
            loadComponent: () =>
              import('./Components/admin/admin-dashboard/admin-dashboard').then(
                (m) => m.AdminDashboard,
              ),
          },
          {
            path: 'approvals',
            loadComponent: () =>
              import('./Components/admin/approval-dashboard/approval-dashboard').then(
                (m) => m.ApprovalDashboard,
              ),
          },
          {
            path: 'users',
            loadComponent: () =>
              import('./Components/admin/admin-dashboard/admin-dashboard').then(
                (m) => m.AdminDashboard,
              ),
          },
          {
            path: 'manage-user/:userId',
            loadComponent: () =>
              import('./Components/admin/manage-user/manage-user').then((m) => m.ManageUser),
          },
          {
            path: 'manage-schedule-contracts/:addressId',
            loadComponent: () =>
              import('./Components/admin/manage-schedule-contracts/manage-schedule-contracts').then(
                (m) => m.ManageScheduleContracts,
              ),
          },
        ],
      },

      // Driver routes
      {
        path: 'driver',
        canActivate: [roleGuard(UserRoles.Driver)],
        loadComponent: () =>
          import('./Components/driver/driver-dashboard/driver-dashboard').then(
            (m) => m.DriverDashboard,
          ),
      },

      // Customer routes
      {
        path: 'customer',
        canActivate: [roleGuard(UserRoles.Customer)],
        children: [
          {
            path: '',
            loadComponent: () =>
              import('./Components/customer/customer-dashboard/customer-dashboard').then(
                (m) => m.CustomerDashboard,
              ),
          },
          {
            path: 'addresses',
            loadComponent: () =>
              import('./Components/customer/my-addresses/my-addresses').then((m) => m.MyAddresses),
          },
          {
            path: 'addresses/new',
            loadComponent: () =>
              import('./Components/customer/add-address/add-address').then(
                (m) => m.CustomerAddAddress,
              ),
          },
          {
            path: 'addresses/:addressId',
            loadComponent: () =>
              import('./Components/customer/address-detail/address-detail').then(
                (m) => m.AddressDetail,
              ),
          },
          {
            path: 'profile',
            loadComponent: () =>
              import('./Components/customer/edit-profile/edit-profile').then((m) => m.EditProfile),
          },
        ],
      },

      // Default redirect for authenticated users
      {
        path: '',
        redirectTo: '/login',
        pathMatch: 'full',
      },
    ],
  },

  // Fallback
  {
    path: '**',
    redirectTo: '/login',
  },
];
