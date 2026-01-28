import {inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {catchError, map, Observable, of, tap} from 'rxjs';
import {AuthenticationState} from '../core/interfaces/authentication-state.interface';
import {environment} from '../../environments/environment';
import {UserInfo} from '../core/interfaces/userinfo.interface';
import {User} from '../core/interfaces/user.interface';
import {AuthenticationStateProvider} from './authentication-state-provider.service';
import {AuthenticationChannel} from './authentication-channel.service';
import {AuthenticationEvents} from './authentication-events';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  private httpClient = inject(HttpClient);
  private channel = inject(AuthenticationChannel);
  private state = inject(AuthenticationStateProvider);

  public getMe = (): Observable<void> => {
    return this.httpClient
      .get<UserInfo>(`${environment.backendUri}/api/v1/connect/userinfo`)
      .pipe(
        tap(user => {
          this.state.signIn(user);
        }),
        catchError((error) => {
          console.log(error.error);
          this.state.signOut();
          return of(void 0)
        }),
        map(() => void 0));
  }

  public logout = () => {
    this.channel.postMessage(AuthenticationEvents.LOGOUT);
    document.location.href = `${environment.backendUri}/bff/logout`;
  }

  public login = () => {
    document.location.href = `${environment.backendUri}/bff/login`;
  }
  public register = () => {
    document.location.href = `${environment.registerUri}?from=eCinema`;
  }
}
