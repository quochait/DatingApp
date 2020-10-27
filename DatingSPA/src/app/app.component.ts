import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();

  title = 'DatingSPA';

  constructor(
    private authService: AuthService
    ) {}

  ngOnInit() {
    const token = localStorage.getItem('token');
    const user = JSON.parse(localStorage.getItem('user'));

    if (token) {
      this.authService.decodeToken = this.jwtHelper.decodeToken(token);
    }

    if (user) {
      console.log(user.mainPhotoUrl);
      this.authService.changeMainPhoto(user.mainPhotoUrl);
    }
  }
}
