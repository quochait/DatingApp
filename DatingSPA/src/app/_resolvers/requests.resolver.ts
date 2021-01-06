import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';
import { UserService } from '../_services/user.service';
import { RelationshipService } from '../_services/relationship.service';

@Injectable({ providedIn: 'root' })
export class RequestsResolver implements Resolve<any> {
  resolve(): Observable<any> {
    
    return this.relationshipService.getListPending().pipe(
      catchError(() => {
        this.alertifyService.error("Can't not get requests pending.");
        return of(null);
      })
    );
  }

  constructor(
    private alertifyService: AlertifyService,
    private relationshipService: RelationshipService
  ) {}
}