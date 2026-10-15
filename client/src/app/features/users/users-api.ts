import { inject, Service } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { DirectoryUser } from './user';
import { HttpClient } from '@angular/common/http';

@Service()
export class UsersApi {
  private readonly httpClient = inject(HttpClient);
  private readonly BASE_URL = `${environment.apiUrl}/api/users`;

  getUsers(): Observable<DirectoryUser[]> {
    return this.httpClient.get<DirectoryUser[]>(this.BASE_URL);
  }
}
