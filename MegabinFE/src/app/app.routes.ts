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
    path: '',
    redirectTo: '/login',
    pathMatch: 'full',
  },
];
