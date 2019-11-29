import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class MemberListResolver implements Resolve<User[]> {
  pageNumber = 1;
  pageSize = 10;

  resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
    return this.userService.getUsers(this.pageNumber, this.pageSize).pipe(
      catchError(() => {
        this.alertify.error('Problem retrieving your data');
        this.router.navigate(['']);
        return of(null);
      })
    );
  }
  constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) {}
}
