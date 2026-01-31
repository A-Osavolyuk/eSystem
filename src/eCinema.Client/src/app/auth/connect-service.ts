import {inject, Injectable} from '@angular/core';
import {catchError, EMPTY, Observable, of, tap} from 'rxjs';
import {environment} from '../../environments/environment';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ConnectService {
  private httpClient = inject(HttpClient);

  public frontchannelLogout = (): Observable<void> => {
    return this.httpClient.get<void>(`${environment.backendUri}/connect/frontchannel-logout`)
      .pipe(
        catchError((error) => {
          console.log(error.error);
          return EMPTY
        }));
  }
}
