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
    path: '',
    redirectTo: '/login',
    pathMatch: 'full',
  },
];
