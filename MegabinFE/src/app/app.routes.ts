import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./Components/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    loadComponent: () => import('./Components/register/register').then((m) => m.Register),
  },
  {
    path: 'autocomplete',
    loadComponent: () =>
      import('./Components/autocomplete/autocomplete').then((m) => m.Autocomplete),
  },
  {
    path: 'admin/admin-dashboard',
    loadComponent: () =>
      import('./Components/admin/admin-dashboard/admin-dashboard').then((m) => m.AdminDashboard),
  },
  {
    path: 'admin/manage-user/:userId',
    loadComponent: () =>
      import('./Components/admin/manage-user/manage-user').then((m) => m.ManageUser),
  },
  {
    path: 'admin/manage-schedule-contracts/:addressId',
    loadComponent: () =>
      import('./Components/admin/manage-schedule-contracts/manage-schedule-contracts').then(
        (m) => m.ManageScheduleContracts,
      ),
  },
  // Driver routes
  {
    path: 'driver',
    loadComponent: () =>
      import('./Components/driver/driver-dashboard/driver-dashboard').then(
        (m) => m.DriverDashboard,
      ),
  },
  // Customer routes
  {
    path: 'customer',
    loadComponent: () =>
      import('./Components/customer/customer-dashboard/customer-dashboard').then(
        (m) => m.CustomerDashboard,
      ),
  },
  {
    path: 'customer/addresses',
    loadComponent: () =>
      import('./Components/customer/my-addresses/my-addresses').then((m) => m.MyAddresses),
  },
  {
    path: 'customer/addresses/new',
    loadComponent: () =>
      import('./Components/customer/add-address/add-address').then((m) => m.CustomerAddAddress),
  },
  {
    path: 'customer/addresses/:addressId',
    loadComponent: () =>
      import('./Components/customer/address-detail/address-detail').then((m) => m.AddressDetail),
  },
  {
    path: 'customer/profile',
    loadComponent: () =>
      import('./Components/customer/edit-profile/edit-profile').then((m) => m.EditProfile),
  },
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full',
  },
];
