import {Component, inject, OnInit} from '@angular/core';
import {AuthService} from '../../../auth/auth.service';

@Component({
  selector: 'e-frontchannel-logout',
  imports: [],
  templateUrl: './frontchannel-logout-page.html',
  styleUrl: './frontchannel-logout-page.scss',
})
export class FrontchannelLogoutPage implements OnInit {
  private auth = inject(AuthService);

  ngOnInit(): void {
    this.auth.frontchannelLogout().subscribe(
      () => console.log('front-channel logout successful'),
    )
  }

}
