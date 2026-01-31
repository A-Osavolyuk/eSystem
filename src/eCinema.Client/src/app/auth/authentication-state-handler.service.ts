import {effect, inject, Injectable, OnDestroy} from '@angular/core';
import {AuthenticationEvents} from './authentication-events';
import {AuthenticationChannel} from './authentication-channel.service';
import {AuthenticationStateProvider} from './authentication-state-provider.service';
import {AuthenticationStateNotifier} from './authentication-state-notifier.service';
import {AuthenticationService} from './authentication.service';
import {catchError, EMPTY, tap} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationStateHandler {
  private channel = inject(AuthenticationChannel);
  private stateProvider = inject(AuthenticationStateProvider);
  private notifier = inject(AuthenticationStateNotifier);
  private auth = inject(AuthenticationService);

  constructor() {
    effect(() => {
      if (this.stateProvider.state().isAuthenticated) {
        this.notifier.connect();
      } else {
        this.notifier.disconnect();
      }
    });
  }

  public listenEvents = () => {
    this.channel.getChannel().onmessage = (message) => {
      if (message.data.type === AuthenticationEvents.LOGOUT) {
        console.log("Received logout event from another tab");
        if (this.stateProvider.state().isAuthenticated) {
          console.log("Signing out...");
          this.stateProvider.signOut();
          console.log("Signed out successfully");
        }
      } else if (message.data.type === AuthenticationEvents.LOGIN) {
        console.log("Received login event from another tab");
        if (!this.stateProvider.state().isAuthenticated) {
          console.log("Signing in...");
          this.auth.getMe()
            .pipe(
              tap(userInfo => {
                this.stateProvider.signIn(userInfo);
              }),
              catchError(() => {
                console.log("Error on signing in");
                return EMPTY;
              })
            )
            .subscribe(() => console.log("Logged in successfully"));
        }
      }
    }
  }
}
