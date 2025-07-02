import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginRequest } from '../requests/loginRequest';
import { ResponseDto } from '../dtos/responseDto';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  private gatewayUri = environment.gatewayUri;

  constructor(private http: HttpClient) {}

  login(req: LoginRequest): Observable<ResponseDto> {
    return this.http.post<ResponseDto>(`${this.gatewayUri}/api/v1/Auth/login`, req);
  }
}
