import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginRequest } from '../../../../shared/requests/loginRequest';
import { AuthenticationService } from '../../../../shared/services/authentication.service';
import { ResponseDto } from '../../../../shared/dtos/responseDto';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  public request : LoginRequest = new LoginRequest("sasha.osavolll111@gmail.com", "Test_1234");

  constructor(private router: Router, private auth : AuthenticationService) {}

  onLoginClick(){
    this.auth.login(this.request).subscribe(
      (response : ResponseDto ) => {
        console.log(response.resultMessage, response.result)
      },
      error => {
        console.error('There was an error during the request', error);
      }
    );
  }
}
