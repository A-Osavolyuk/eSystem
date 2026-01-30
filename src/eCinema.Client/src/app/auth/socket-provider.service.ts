import { Injectable } from '@angular/core';
import {environment} from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SocketProvider {
  public getSocket() : WebSocket {
    return new WebSocket(environment.backendSocketUri);
  }
}
