import {Component, inject, OnInit} from '@angular/core';
import {ConnectService} from '../../../auth/connect-service';
import {AuthenticationEvents} from '../../../auth/authentication-events';
import {AuthenticationChannel} from '../../../auth/authentication-channel.service';
import {AuthenticationStateProvider} from '../../../auth/authentication-state-provider.service';

@Component({
  selector: 'e-frontchannel-logout',
  imports: [],
  templateUrl: './frontchannel-logout-page.html',
  styleUrl: './frontchannel-logout-page.scss',
})
export class FrontchannelLogoutPage implements OnInit {
  private connectService = inject(ConnectService);
  private state = inject(AuthenticationStateProvider);
  private channel = inject(AuthenticationChannel);

  ngOnInit(): void {
    this.connectService.frontchannelLogout()
      .subscribe(() => {
          this.state.signOut();
          this.channel.postMessage(AuthenticationEvents.LOGOUT);
        }
      )
  }

}
