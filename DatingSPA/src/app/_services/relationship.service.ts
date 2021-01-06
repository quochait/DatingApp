import { HttpClient } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Relationship } from '../_models/relationship';
import { AuthService } from './auth.service';

@Injectable({providedIn: 'root'})
export class RelationshipService {
  baseUrl = environment.apiUrl + 'user';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ){}
  
  makeRequestMatch(userId: string){
    let form = new FormData();
    form.append('userDest', userId);
    return this.http.post(this.baseUrl + '/' + this.authService.decodeToken.nameid + '/getMatch', form);
  }

  getStatusRelationship(userId: string): Observable<Relationship>{
    let form = new FormData();
    form.append('userDest', userId);
    return this.http.post<Relationship>(this.baseUrl + '/' + this.authService.decodeToken.nameid + '/getStatusRelationship', form);
  }

  getListPending(){
    return this.http.get(this.baseUrl + '/getRequestsPending');
  }
}