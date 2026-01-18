import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
  selector: 'app-button',
  imports: [],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class Button {
    @Input() public label: string | undefined;
    @Output() public onClick : EventEmitter<void> = new EventEmitter();
}
