import {inject, Injectable, NgZone} from '@angular/core';
import {AuthenticationStateProvider} from './authentication-state-provider.service';
import {WsMessage} from '../core/interfaces/ws-message';
import {AuthenticationEvents} from './authentication-events';
import {AuthenticationChannel} from './authentication-channel.service';
import {SocketProvider} from './socket-provider.service';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationStateNotifier {
  private state = inject(AuthenticationStateProvider)
  private socketProvider = inject(SocketProvider)
  private zone = inject(NgZone)

  private socket?: WebSocket;
  private reconnectDelay = 1000;

  public connect(): void {
    if (this.isSocketConnected()) return;

    this.socket = this.socketProvider.getSocket();

    this.socket.onopen = () => {
      console.log('WS connected');
      this.reconnectDelay = 1000;
    };

    this.socket.onmessage = (e) => {
      this.zone.run(() => this.handleMessage(e));
    };

    this.socket.onclose = () => {
      console.warn('WS closed');
      this.socket = undefined;
      this.reconnect();
    };

    this.socket.onerror = (e) => {
      console.error('WS error', e);
    };
  }

  public disconnect(): void {
    if (!this.isSocketConnected() || !this.socket) return;

    try {
      this.socket.close();
    } catch (err) {
      console.error('WS disconnect error', err);
    } finally {
      this.socket = undefined;
    }
  }

  private handleMessage(e: MessageEvent) : void {
    const message = JSON.parse(e.data) as WsMessage;

    if (message.event === AuthenticationEvents.LOGOUT) {
      this.state.signOut();
    }
  }

  private reconnect() : void {
    this.disconnect();
    setTimeout(() => this.connect(), this.reconnectDelay);
    this.reconnectDelay = Math.min(this.reconnectDelay * 2, 30000);
  }

  private isSocketConnected() : boolean {
    return !!this.socket &&
      (this.socket.readyState === WebSocket.OPEN ||
      this.socket.readyState === WebSocket.CONNECTING);
  }
}
