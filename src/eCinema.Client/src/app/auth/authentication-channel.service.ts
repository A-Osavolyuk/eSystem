import {Injectable, OnDestroy} from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationChannel implements OnDestroy {
  private channel = new BroadcastChannel('authentication');

  public postMessage(message: string): void;
  public postMessage<TPayload>(message: string, payload: TPayload): void;

  public postMessage<TPayload>(message: string, payload?: TPayload): void {
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
