import {Component, inject} from '@angular/core';
import {RouterLink} from '@angular/router';
import {Button} from '../../../common/button/button';
import {AuthenticationOutlet} from '../authentication-outlet/authentication-outlet';

@Component({
  selector: 'e-main-layout-header',
  imports: [
    Button,
    RouterLink,
    AuthenticationOutlet
  ],
  templateUrl: './main-layout-header.html',
  styleUrl: './main-layout-header.scss',
})
export class MainLayoutHeader {}
