import {Component, inject} from '@angular/core';
import {AuthenticationService} from '../../../../auth/authentication.service';
import {Button} from '../../../common/button/button';
import {environment} from '../../../../../environments/environment';
import {AuthenticationStateProvider} from '../../../../auth/authentication-state-provider.service';

@Component({
  selector: 'e-authentication-outlet',
  imports: [
    Button
  ],
  templateUrl: './authentication-outlet.html',
  styleUrl: './authentication-outlet.scss',
})
export class AuthenticationOutlet {
  protected stateProvider = inject(AuthenticationStateProvider)
  protected auth = inject(AuthenticationService);
}
