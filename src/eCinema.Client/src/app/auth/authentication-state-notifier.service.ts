import {inject, Injectable} from '@angular/core';
import {AuthenticationStateProvider} from './authentication-state-provider.service';
import {HubConnection, HubConnectionBuilder} from '@microsoft/signalr';
import {environment} from '../../environments/environment';
import {catchError, EMPTY, from} from 'rxjs';
import {ConnectService} from './connect-service';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationStateNotifier {
  private state = inject(AuthenticationStateProvider)
  private connectService = inject(ConnectService);
  private hubConnection?: HubConnection;

  public connect(): void {
    if (this.hubConnection) {
      return;
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.backendSocketUri, {
        withCredentials: true,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.onreconnecting(error => {
      console.warn("SignalR reconnecting...", error);
    });

    this.hubConnection.onreconnected(connectionId => {
      console.log("SignalR reconnected", connectionId);
    });

    this.hubConnection.onclose(error => {
      console.error("SignalR closed", error);
    });

    this.hubConnection.on("Logout", () => {
      console.log("Global logout received");
      this.connectService.frontchannelLogout()
        .subscribe(() => {
          this.state.signOut();
          this.disconnect();
        });
    });

    from(this.hubConnection.start())
      .pipe(
        catchError(error => {
          console.error(error)
          return EMPTY
        })
      )
      .subscribe(() => {
        console.log("Connected to SignalR hub")
      });
  }

  public disconnect(): void {
    if (this.hubConnection) {
      from(this.hubConnection.stop())
        .pipe(
          catchError(error => {
            console.error(error)
            return EMPTY
          })
        )
        .subscribe(() => {
          console.log("Close connection with SignalR hub")
          this.hubConnection = undefined;
        })
    }
  }
}
