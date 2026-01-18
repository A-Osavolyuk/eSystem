import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../../environments/environment';
import {User} from '../interfaces/user.interface';
import {catchError, Observable, of} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private httpClient = inject(HttpClient)

  public getMe = () : Observable<User | null> => {
    return this.httpClient.get<User | null>(`${environment.backendUri}/api/v1/authentication/me`)
      .pipe(
        catchError((error) => {
          console.error('[AuthService] getMe error', error);
          return of(null);
      }));
  }

  public login = () : void => {
    document.location.href = `${environment.backendUri}/api/v1/connect/authorize`;
  }

  public register = () : void => {
    document.location.href = `${environment.registerUri}?from=eCinema`;
  }

  public logout = () : void => {
    document.location.href = `${environment.backendUri}/api/v1/connect/logout`;
  }
}
