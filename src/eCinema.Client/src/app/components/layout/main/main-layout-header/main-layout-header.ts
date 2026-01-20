import {Component, inject} from '@angular/core';
import {RouterLink} from '@angular/router';
import {Button} from '../../../common/button/button';
import {AuthService} from '../../../../auth/auth.service';

@Component({
  selector: 'e-main-layout-header',
  imports: [
    Button,
    RouterLink
  ],
  templateUrl: './main-layout-header.html',
  styleUrl: './main-layout-header.scss',
})
export class MainLayoutHeader {
  protected authService = inject(AuthService)
}
