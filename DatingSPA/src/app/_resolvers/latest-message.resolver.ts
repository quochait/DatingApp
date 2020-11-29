import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { Observable, of } from 'rxjs';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';
import { Message } from '../_models/message';
import { MessageService } from '../_services/message.service';
import { AuthService } from '../_services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class LatestMessageResolver implements Resolve<Message> {
  resolve(route: ActivatedRouteSnapshot): Observable<Message> {
    return this.messageService.getLatestMessage(this.authService.decodeToken.nameid).pipe(
      catchError((error) => {
        this.alertify.error('Problem retrieving your data');
        return of(null);
      })
    )
  }

  constructor(
    private messageService: MessageService, 
    private authService: AuthService,
    private alertify: AlertifyService,
  ) { }
}
