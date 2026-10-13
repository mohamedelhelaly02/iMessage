import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { RegisterRequest } from '../models/register-request';
import { AuthResponse } from '../models/auth-response';
import { LoginCredentails } from '../models/credentails';

@Service()
export class AuthApi {
  private readonly BASE_URL: string = `${environment.apiUrl}/api/auth`;
  private readonly httpClient = inject(HttpClient);

  register(registerData: RegisterRequest): Observable<void> {
    return this.httpClient.post<void>(`${this.BASE_URL}/register`, registerData);
  }

  login(loginData: LoginCredentails): Observable<AuthResponse> {
    return this.httpClient.post<AuthResponse>(`${this.BASE_URL}/login`, loginData);
  }

  logout(): Observable<void> {
    return this.httpClient.post<void>(`${this.BASE_URL}/logout`, {});
  }

  refresh(): Observable<AuthResponse> {
    return this.httpClient.post<AuthResponse>(`${this.BASE_URL}/refresh`, {});
  }
}
