import { Routes } from '@angular/router';
import { authGuard } from './core/guard/auth-guard';
import { guestGuard } from './core/guard/guest-guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./core/layouts/blank/blank').then((c) => c.Blank),
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'conversations', pathMatch: 'full' },
      {
        path: 'conversations',
        loadComponent: () =>
          import('./features/chat/pages/empty/empty').then(
            (c) => c.Empty,
          ),
        title: 'conversations',
      },
      {
        path: 'conversations/:id',
        loadComponent: () =>
          import('./features/chat/pages/conversation/conversation').then((c) => c.Conversation),
        title: 'Details',
      },
    ],
  },
  {
    path: 'auth',
    loadComponent: () => import('./core/layouts/auth/auth').then((c) => c.Auth),
    canActivate: [guestGuard],
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      {
        path: 'login',
        loadComponent: () => import('./features/auth/pages/login/login').then((c) => c.Login),
        title: 'Login',
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./features/auth/pages/register/register').then((c) => c.Register),
        title: 'Register',
      },
    ],
  },
  {
    path: '**',
    loadComponent: () => import('./shared/not-fount/not-fount').then((c) => c.NotFount),
    title: '404 not-found',
  },
];
