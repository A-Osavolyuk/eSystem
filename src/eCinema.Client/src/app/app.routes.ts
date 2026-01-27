import {Routes} from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/layout/main/main-layout/main-layout')
        .then(c => c.MainLayout),
    children: [
      {
        path: '',
        loadComponent: () => import('./pages/home/home-page.component')
          .then(c => c.HomePage),
      },
      {
        path: 'connect/error',
        loadComponent: () => import('./pages/connect/error-page/error-page')
          .then(c => c.ErrorPage),
      },
      {
        path: 'connect/logged-out',
        loadComponent: () => import('./pages/connect/logged-out-page/logged-out-page')
          .then(c => c.LoggedOutPage),
      },
      {
        path: 'connect/frontchannel-logout',
        loadComponent: () => import('./pages/connect/frontchannel-logout/frontchannel-logout-page')
          .then(c => c.FrontchannelLogoutPage),
      }
    ]
  }
];
