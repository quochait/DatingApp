import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Relationship } from '../_models/relationship';
import { AlertifyService } from '../_services/alertify.service';
import { RelationshipService } from '../_services/relationship.service';

@Injectable({providedIn: 'root'})
export class RelationshipResolver implements Resolve<Relationship> {
  resolve(route: ActivatedRouteSnapshot) : Observable<Relationship> {
    return this.relationshipService.getStatusRelationship(route.paramMap.get('id')).pipe(
      catchError(() => {
        this.alertify.error('Problem retrieving your data');
        return of(null);
      })
    );
  }

  constructor(
    private relationshipService: RelationshipService,
    private alertify: AlertifyService
  ){}
}