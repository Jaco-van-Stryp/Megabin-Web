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
    path: 'admin',
    loadComponent: () =>
      import('./Components/admin/admin-dashboard/admin-dashboard').then((m) => m.AdminDashboard),
    children: [
      {
        path: '',
        redirectTo: 'users',
        pathMatch: 'full',
      },
      {
        path: 'users',
        loadComponent: () =>
          import('./Components/admin/user-list/user-list').then((m) => m.UserList),
      },
      {
        path: 'users/:userId',
        loadComponent: () =>
          import('./Components/admin/user-detail/user-detail').then((m) => m.UserDetail),
      },
      {
        path: 'users/:userId/addresses/:addressId',
        loadComponent: () =>
          import('./Components/admin/address-detail/address-detail').then((m) => m.AddressDetail),
      },
    ],
  },
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full',
  },
];
