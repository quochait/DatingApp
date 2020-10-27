import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  user: User;

  constructor(
    public authService: AuthService, 
    private alertify: AlertifyService, 
    private router: Router,
    private userService: UserService
    ) {}

  ngOnInit() {
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (user) {
      this.user = user;
      console.log(this.user);
      // this.authService.currentMainPhoto.subscribe(res => {
      //   this.user.mainPhotoUrl = res;
      // });
    }
  }

  login() {
    this.authService.login(this.model).subscribe(
      next => {
        this.alertify.success('Logged in successfully.');
        this.user = JSON.parse(localStorage.getItem('user'));
        // console.log(this.user);
      },
      error => {
        this.alertify.error(error);
      },
      () => {
        this.router.navigate(['/members']);
      }
    );
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    this.authService.logout();
    this.alertify.message('Logout.');
    this.router.navigate(['/home']);
  }
}
