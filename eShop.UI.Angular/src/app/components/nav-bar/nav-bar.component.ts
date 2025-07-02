import { Component, Input } from '@angular/core';
import { EventHandlerService } from '../../../shared/services/event-handler.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.css'
})
export class NavBarComponent {
  private isClosed: boolean = false;
  constructor(private events: EventHandlerService){}

  toogleSidebar(){
    this.isClosed = !this.isClosed;
    this.events.emit("toogle-sidebar", this.isClosed)
  }
}
