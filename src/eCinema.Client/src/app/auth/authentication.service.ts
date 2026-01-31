import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {catchError, EMPTY, map, Observable, of, tap} from 'rxjs';
import {environment} from '../../environments/environment';
import {UserInfo} from '../core/interfaces/userinfo.interface';
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

  public getMe = (): Observable<UserInfo> => {
    return this.httpClient.get<UserInfo>(`${environment.backendUri}/api/v1/connect/userinfo`);
  }

  public authenticate = (): Observable<void> => {
    return this.getMe()
      .pipe(
        tap(user => {
          this.state.signIn(user);
          this.channel.postMessage(AuthenticationEvents.LOGIN);
        }),
        catchError((error) => {
          console.log(error.error);
          this.channel.postMessage(AuthenticationEvents.LOGOUT);
          return of(void 0)
        }),
        map(() => void 0))
  }

  public logout = (): void => {
    this.state.signOut();
    this.channel.postMessage(AuthenticationEvents.LOGOUT);
    document.location.href = `${environment.backendUri}/bff/logout`;
  }

  public login = (): void => {
    document.location.href = `${environment.backendUri}/bff/login`;
  }
  public register = (): void => {
    document.location.href = `${environment.registerUri}?from=eCinema`;
  }
}
