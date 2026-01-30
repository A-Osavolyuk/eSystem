import {effect, inject, Injectable, OnDestroy} from '@angular/core';
import {AuthenticationEvents} from './authentication-events';
import {AuthenticationChannel} from './authentication-channel.service';
import {AuthenticationStateProvider} from './authentication-state-provider.service';
import {UserInfo} from '../core/interfaces/userinfo.interface';
import {AuthenticationStateNotifier} from './authentication-state-notifier.service';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationStateHandler {
  private channel = inject(AuthenticationChannel);
  private auth = inject(AuthenticationStateProvider);
  private notifier = inject(AuthenticationStateNotifier);

  constructor() {
    effect(() => {
      if (this.auth.state().isAuthenticated) {
        this.notifier.connect();
      } else {
        this.notifier.disconnect();
      }
    });
  }

  public listenEvents = () => {
    this.channel.getChannel().onmessage = (message) => {
      if (message.data.type === AuthenticationEvents.LOGOUT) {
        console.log("Received logout event from another tab, signing out...");
        this.auth.signOut();
      } else if (message.data.type === AuthenticationEvents.LOGIN) {
        console.log("Received login event from another tab, signing in...");
        this.auth.signIn(message.data.payload as UserInfo);
      }
    }
  }
}
