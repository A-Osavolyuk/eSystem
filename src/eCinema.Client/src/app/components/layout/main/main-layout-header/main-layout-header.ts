import {Component, inject} from '@angular/core';
import {Router} from '@angular/router';
import {AuthService} from '../../../../core/services/auth.service';
import {Button} from '../../../common/button/button';

@Component({
  selector: 'app-main-layout-header',
  imports: [
    Button
  ],
  templateUrl: './main-layout-header.html',
  styleUrl: './main-layout-header.scss',
})
export class MainLayoutHeader {
  private router = inject(Router)
  protected authService = inject(AuthService)

  protected onBrandClick = () => {
    this.router.navigate(['/app']);
  }
}
