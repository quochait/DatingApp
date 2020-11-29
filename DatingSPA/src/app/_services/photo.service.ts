import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { FileItem } from 'ng2-file-upload';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Photo } from '../_models/Photo';

@Injectable({
	providedIn: 'root'
})

export class PhotoService {
	baseUrl = environment.apiUrl + 'photo';
	constructor(private http: HttpClient, private authService: AuthService) { }

	uploadPhoto(form: FormData): Observable<any> {
		return this.http.post(this.baseUrl + '/' + this.authService.decodeToken.nameid + '/photos', form);
	}

	getPhotos() {
		return this.http.get<Photo[]>(this.baseUrl + '/' + this.authService.decodeToken.nameid + '/photos' + '/getPhotos');
	}

	getPhotosDetail(userId: string){
		return this.http.get<Photo[]>(this.baseUrl + '/' + userId + '/photos' + '/getPhotos');
	}
}