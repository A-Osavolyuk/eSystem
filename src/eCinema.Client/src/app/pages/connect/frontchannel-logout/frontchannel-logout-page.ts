import {Component, inject, OnInit} from '@angular/core';
import {AuthenticationService} from '../../../auth/authentication.service';
import {ConnectService} from '../../../auth/connect-service';

@Component({
  selector: 'e-frontchannel-logout',
  imports: [],
  templateUrl: './frontchannel-logout-page.html',
  styleUrl: './frontchannel-logout-page.scss',
})
export class FrontchannelLogoutPage implements OnInit {
  private connectService = inject(ConnectService);

  ngOnInit(): void {
    this.connectService.frontchannelLogout().subscribe(
      () => console.log('front-channel logout successful'),
    )
  }

}
