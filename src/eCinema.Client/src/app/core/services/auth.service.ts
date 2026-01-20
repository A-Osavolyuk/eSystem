import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../../environments/environment';
import {User} from '../interfaces/user.interface';
import {catchError, Observable, of} from 'rxjs';
import {UserInfo} from '../interfaces/userinfo.interface';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private httpClient = inject(HttpClient)

  public getMe = () : Observable<UserInfo | null> => {
    return this.httpClient.get<UserInfo>(`${environment.backendUri}/api/v1/connect/userinfo`)
      .pipe(
        catchError((error) => {
          console.error('[AuthService] getMe error', error);
          return of(null);
      }));
  }

  public login = () : void => {
    document.location.href = `${environment.backendUri}/api/authentication/login`;
  }

  public register = () : void => {
    document.location.href = `${environment.registerUri}?from=eCinema`;
  }

  public logout = () : void => {
    document.location.href = `${environment.backendUri}/api/authentication/logout`;
  }
}
