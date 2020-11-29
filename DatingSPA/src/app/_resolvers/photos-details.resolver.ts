import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Photo } from '../_models/Photo';
import { AlertifyService } from '../_services/alertify.service';
import { PhotoService } from '../_services/photo.service';

@Injectable(
  {
    providedIn: 'root'
  }
)
export class PhotosDetailResolver implements Resolve<Photo[]>{
  resolve(route: ActivatedRouteSnapshot) : Observable<Photo[]> {
    return this.photoService.getPhotosDetail(route.paramMap.get('id')).pipe(
      catchError(error => {
        this.alertify.error('Can not get photos.');
        console.log(route.paramMap.get('id'))
        return of(null);
      })
    );
  }

  constructor(
    private photoService: PhotoService,
    private alertify: AlertifyService
  ){}
}