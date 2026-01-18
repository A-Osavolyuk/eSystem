import {Component} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {MainLayoutSidebar} from '../main-layout-sidebar/main-layout-sidebar';
import {MainLayoutHeader} from '../main-layout-header/main-layout-header';

@Component({
  selector: 'e-main-layout',
  imports: [
    RouterOutlet,
    MainLayoutSidebar,
    MainLayoutHeader
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss',
})
export class MainLayout {

}
