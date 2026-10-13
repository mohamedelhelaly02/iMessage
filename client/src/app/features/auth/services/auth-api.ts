import { HttpClient, HttpContext } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SKIP_CREDENTIALS } from '../../../core/auth.tokens';
import { RegisterRequest } from '../models/register-request';
import { AuthResponse } from '../models/auth-response';
import { LoginCredentails } from '../models/credentails';

@Service()
export class AuthApi {
  private readonly BASE_URL: string = `${environment.apiUrl}/api/auth`;
  private readonly httpClient = inject(HttpClient);

  register(registerData: RegisterRequest): Observable<void> {
    return this.httpClient.post<void>(`${this.BASE_URL}/register`, registerData, {
      context: new HttpContext().set(SKIP_CREDENTIALS, true),
    });
  }

  login(loginData: LoginCredentails): Observable<AuthResponse> {
    return this.httpClient.post<AuthResponse>(`${this.BASE_URL}/login`, loginData, {
      context: new HttpContext().set(SKIP_CREDENTIALS, true),
    });
  }

  logout(): Observable<void> {
    return this.httpClient.post<void>(`${this.BASE_URL}/logout`, {});
  }

  refresh(): Observable<AuthResponse> {
    return this.httpClient.post<AuthResponse>(`${this.BASE_URL}/refresh`, {});
  }
}
