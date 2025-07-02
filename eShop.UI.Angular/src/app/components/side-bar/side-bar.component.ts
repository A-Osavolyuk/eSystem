import { Component, Input } from '@angular/core';
import { EventHandlerService } from '../../../shared/services/event-handler.service';

@Component({
  selector: 'app-side-bar',
  templateUrl: './side-bar.component.html',
  styleUrl: './side-bar.component.css'
})
export class SideBarComponent {
  private isClosed: boolean = false;

  constructor(private events: EventHandlerService){
    events.listen("toogle-sidebar", (state:boolean) => {
      this.isClosed = state;
    })
  }

  get cssClasses(){
    return !this.isClosed ? ['sidebar-opened'] : ['sidebar-closed']
  }
}
