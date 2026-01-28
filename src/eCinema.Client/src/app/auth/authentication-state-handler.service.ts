import {inject, Injectable} from '@angular/core';
import {AuthenticationEvents} from './authentication-events';
import {AuthenticationChannel} from './authentication-channel.service';
import {AuthenticationStateProvider} from './authentication-state-provider.service';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationStateHandler {
  private channel = inject(AuthenticationChannel);
  private auth = inject(AuthenticationStateProvider);

  public listenEvents = () => {
    this.channel.getChannel().onmessage = (message) => {
      if (message.data === AuthenticationEvents.LOGOUT) {
        console.log("Received logout event from another tab, signing out...");
        this.auth.signOut();
      }
    }
  }
}
