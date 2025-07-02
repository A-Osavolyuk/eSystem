import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EventHandlerService {
  private subject = new Subject();
  constructor() { }

  emit(eventName: string, payload: any){
    this.subject.next({eventName, payload});
  }

  listen(eventName: string, callback: (event: any) => void){
    this.subject.asObservable().subscribe((nextObject: any) => {
      if(eventName === nextObject.eventName){
        callback(nextObject.payload);
      }
    })
  }
}
