import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationChannel {
  private channel = new BroadcastChannel('authentication');

  public postMessage = (message: string): void => {
    this.channel.postMessage(message);
  }

  public getChannel = (): BroadcastChannel => this.channel;
}
