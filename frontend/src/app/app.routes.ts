import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { adminGuard } from './core/auth/admin.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/auth/login/login').then(m => m.Login) },
  { path: 'register', loadComponent: () => import('./features/auth/register/register').then(m => m.Register) },
  {
    path: '',
    loadComponent: () => import('./shared/layout/layout').then(m => m.Layout),
    canActivate: [authGuard],
    children: [
      { path: '', loadComponent: () => import('./features/feed/feed').then(m => m.Feed) },
      { path: 'leaderboard', loadComponent: () => import('./features/leaderboard/leaderboard').then(m => m.Leaderboard) },
      { path: 'profile', loadComponent: () => import('./features/profile/profile').then(m => m.Profile) },
      { path: 'notifications', loadComponent: () => import('./features/notifications/notifications').then(m => m.Notifications) },
      { path: 'admin', loadComponent: () => import('./features/admin/admin').then(m => m.Admin), canActivate: [adminGuard] },
    ],
  },
  { path: '**', redirectTo: '' },
];
