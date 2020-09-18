import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from './../../environments/environment';
import { BehaviorSubject } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodeToken: any;
  photoUrl = new BehaviorSubject<string>('../../assets/default.png');
  currentMainPhoto = this.photoUrl.asObservable();

  constructor(private http: HttpClient) {}

  login(model: User) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          this.decodeToken = this.jwtHelper.decodeToken(user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          // console.log(JSON.stringify(user));
          this.changeMainPhoto(user.user.photoUrl);
        }
      })
    );
  }

  register(user: User) {
    return this.http.post(this.baseUrl + 'register', user);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  logout() {
    localStorage.clear();
    this.decodeToken = null;
  }

  changeMainPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (user) {
      user.photoUrl = photoUrl;
      localStorage.setItem('user', JSON.stringify(user));
    }
  }
}
