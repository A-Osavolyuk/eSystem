import {Component, input, output} from '@angular/core';

@Component({
  selector: 'e-button',
  imports: [],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class Button {
  public readonly label = input<string>();
  public readonly onClick = output();
}
