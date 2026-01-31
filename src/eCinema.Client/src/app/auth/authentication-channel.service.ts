import {Injectable, OnDestroy} from '@angular/core';
import {AuthenticationEvent} from './authentication-events';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationChannel implements OnDestroy {
  private channel = new BroadcastChannel('authentication');

  public postMessage = (message: AuthenticationEvent): void => {
    this.channel.postMessage({ type: message });
  };

  public getChannel = (): BroadcastChannel => this.channel;

  ngOnDestroy(): void {
    this.channel.close();
  }
}
