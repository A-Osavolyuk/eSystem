import {Injectable, OnDestroy} from '@angular/core';
import {AuthenticationEvent} from './authentication-events';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationChannel implements OnDestroy {
  private channel = new BroadcastChannel('authentication');

  public postMessage(message: AuthenticationEvent): void;
  public postMessage<TPayload>(message: AuthenticationEvent, payload: TPayload): void;

  public postMessage<TPayload>(message: AuthenticationEvent, payload?: TPayload): void {
    if (payload === undefined) {
      this.channel.postMessage({ type: message });
    } else {
      this.channel.postMessage({ type: message, payload });
    }
  }

  public getChannel = (): BroadcastChannel => this.channel;

  ngOnDestroy(): void {
    this.channel.close();
  }
}
