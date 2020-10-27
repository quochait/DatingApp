import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';
import { Photo } from '../_models/Photo';
import { PhotoService } from '../_services/photo.service';

@Injectable({ providedIn: 'root' })
export class PhotoUploadResolver implements Resolve<Photo[]> {
  resolve(route: ActivatedRouteSnapshot): Observable<Photo[]> {
    return this.photoService.getPhotos().pipe(catchError((error)=>{
        this.alertify.error('Problem retrieving your data');
        return of(null);
    }))
  }
  constructor(
    private alertify: AlertifyService,
    private photoService: PhotoService) {}
}
