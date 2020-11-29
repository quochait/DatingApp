import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';
import { UserService } from '../_services/user.service';

@Injectable({ providedIn: 'root' })
export class VerifyEmailResolver implements Resolve<any> {
  resolve(route: ActivatedRouteSnapshot): string {
    return route.paramMap.get('token');
  }
  constructor() {}
}
// this.router.navigate(['/member/edit']);