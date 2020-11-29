import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User } from '../_models/user';
import { AlertifyService } from '../_services/alertify.service';
import { MessageService } from '../_services/message.service';

@Injectable({providedIn: 'root'})
export class MessageResolver implements Resolve<User[]>{
  model: any;
  resolve(route: ActivatedRouteSnapshot) : Observable<User[]>{
    // Done
    return this.messageService.getUserMatches().pipe(
      catchError(error => {
        this.alertify.error('Problem retrieving your data');
        return of(null);
      })
    );

    // Test add latest message both users matched
    // return this.messageService.getUserMatches().pipe(res => {
    //   this.model = res;
    //   try {
    //     for (let index = 0; index < this.model.length; index++) {
    //       const user = this.model[index];
    //       this.messageService.getLatestMessage(user.objectId).subscribe((res) => {
    //         console.log('testing.....................');
    //       });
          
    //     }
    //     return this.model;
    //   } catch (error) {
    //     return of(null);
    //   }
    // })
  }

  constructor(
    private messageService: MessageService,
    private alertify: AlertifyService
  ){}
}