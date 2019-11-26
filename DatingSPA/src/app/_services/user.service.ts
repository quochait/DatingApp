import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl + 'user';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl);
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + '/' + id);
  }

  updateUser(user: User) {
    return this.http.put(this.baseUrl + '/' + this.authService.decodeToken.nameid, user);
  }

  setMainPhoto(id: number) {
    return this.http.post(this.baseUrl + '/' + this.authService.decodeToken.nameid + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(id: number) {
    return this.http.delete(this.baseUrl + '/' + this.authService.decodeToken.nameid + '/photos/' + id);
  }
}
