import {Component, inject} from '@angular/core';
import {AuthService} from '../../../../auth/auth.service';
import {Button} from '../../../common/button/button';

@Component({
  selector: 'e-authentication-outlet',
  imports: [
    Button
  ],
  templateUrl: './authentication-outlet.html',
  styleUrl: './authentication-outlet.scss',
})
export class AuthenticationOutlet {
  protected auth = inject(AuthService)
}
