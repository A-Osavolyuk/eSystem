import {inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {catchError, map, Observable, of, tap} from 'rxjs';
import {AuthenticationState} from '../core/interfaces/authentication-state.interface';
import {environment} from '../../environments/environment';
import {UserInfo} from '../core/interfaces/userinfo.interface';
import {User} from '../core/interfaces/user.interface';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private httpClient = inject(HttpClient)
  private authenticationState = signal<AuthenticationState>({
    isAuthenticated: false,
    user: null
  });

  public readonly state = this.authenticationState.asReadonly();

  public getMe = (): Observable<void> => {
    return this.httpClient
      .get<UserInfo>(`${environment.backendUri}/api/v1/connect/userinfo`)
      .pipe(
        map(res => this.mapUserInfo(res)),
        tap(user => {
          this.authenticationState.set({
            isAuthenticated: true,
            user: user
          });
        }),
        catchError((error) => {
          console.log(error.error);
          this.authenticationState.set({
            isAuthenticated: false,
            user: null
          });
          return of(void 0)
        }),
        map(() => void 0 ));
  }

  private mapUserInfo(userInfo: UserInfo) : User {
    return {
      username: userInfo.preferred_username ?? '',
      email: userInfo.email ?? '',
      phone: userInfo.phone,
      address: userInfo.address ? {
        streetAddress: userInfo.address.street_address,
        region: userInfo.address.region,
        countryCode: userInfo.address.country_code,
        locality: userInfo.address.locality,
        postalCode: userInfo.address.postal_code,
      } : null
    }
  }

  public login = (): void => {
    document.location.href = `${environment.backendUri}/api/authentication/login`;
  }

  public register = (): void => {
    document.location.href = `${environment.registerUri}?from=eCinema`;
  }

  public logout = (): void => {
    document.location.href = `${environment.backendUri}/api/authentication/logout`;
  }
}
