import {inject, Injectable} from '@angular/core';
import {catchError, Observable, of, tap} from 'rxjs';
import {environment} from '../../environments/environment';
import {AuthenticationEvents} from './authentication-events';
import {AuthenticationStateProvider} from './authentication-state-provider.service';
import {AuthenticationChannel} from './authentication-channel.service';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ConnectService {
  private state = inject(AuthenticationStateProvider);
  private channel = inject(AuthenticationChannel);
  private httpClient = inject(HttpClient);

  public frontchannelLogout = (): Observable<void> => {
    return this.httpClient.get<void>(`${environment.backendUri}/connect/frontchannel-logout`)
      .pipe(
        tap(() => {
          this.channel.postMessage(AuthenticationEvents.LOGOUT);
          this.state.signOut();
        }),
        catchError((error) => {
          console.log(error.error);
          return of(void 0)
        }));
  }
}
