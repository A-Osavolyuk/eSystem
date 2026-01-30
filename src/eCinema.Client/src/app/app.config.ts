import {ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import {credentialsInterceptor} from './interceptors/credentials-interceptor';
import {AuthenticationService} from './auth/authentication.service';
import {firstValueFrom} from 'rxjs';
import {AuthenticationStateHandler} from './auth/authentication-state-handler.service';
import {AuthenticationStateNotifier} from './auth/authentication-state-notifier.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideAppInitializer(() => {
      const authenticationStateHandler = inject(AuthenticationStateHandler);
      const authenticationService = inject(AuthenticationService);

      authenticationStateHandler.listenEvents();
      return firstValueFrom(authenticationService.getMe());
    }),
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([credentialsInterceptor])),
  ]
};
