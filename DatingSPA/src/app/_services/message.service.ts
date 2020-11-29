import { HttpClient } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { AuthService } from './auth.service';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
//import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { Message } from '../_models/message';

@Injectable({providedIn: "root"})
export class MessageService {
  baseUrl = environment.apiUrl + 'message';

  constructor(
    private http: HttpClient
  ){}

  getUserMatches(){
    return this.http.get<User[]>(this.baseUrl + '/getMatches');  
  }

  getListMessage(toUserId: string){
    return this.http.get<Message[]>(this.baseUrl + '/listMessages/' + toUserId);
  }

  getLatestMessage(toUserId: string){
    console.log("ObjectId in service: " + toUserId);
    console.log(this.baseUrl + '/getLatestMessage/' + toUserId);
    return this.http.get<Message>(this.baseUrl + '/getLatestMessage/' + toUserId);
  }
}