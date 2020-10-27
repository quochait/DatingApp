import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';
import { Photo } from '../_models/Photo';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';

@Injectable({ providedIn: 'root' })
export class VerifyEmailResolver implements Resolve<Photo[]> {
  resolve(route: ActivatedRouteSnapshot): Observable<Photo[]> {
    return this.userService.getUser(this.authService.decodeToken.nameid).pipe(
        catchError((error)=>{
            this.alertify.error('Problem retrieving your data');
            return of(null);
    }));
  }
  constructor(
    private alertify: AlertifyService,
    private authService: AuthService,
    private userService: UserService) {}
}
