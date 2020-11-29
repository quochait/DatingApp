import { HttpClient } from '@angular/common/http';
import { CoreEnvironment } from '@angular/compiler/src/compiler_facade_interface';
import { EventEmitter, Injectable, Output } from "@angular/core";
import { HttpTransportType } from '@aspnet/signalr';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { environment } from '../../environments/environment'
import { Message } from '../_models/message';

@Injectable({ providedIn: 'root' })
export class ChatService {
  hubUrl: string = environment.hubUrl;
  basaApi: string = environment.apiUrl + 'message';

  connectionId: string;
  // initMessage: Message =  new Message();

  messageReceived = new EventEmitter<Message>();
  newLatestMessageReceived = new EventEmitter<Message>();
  connectionEstablished = new EventEmitter<Boolean>();
  

  private connectionIsEstablished = false;
  private _hubConnection: signalR.HubConnection;
  isAuthorized = false;

  constructor(
    private http: HttpClient
  ) {
    this.createConnection();
    this.registerOnServerEvents();
    this.startConnection();
  }

  sendMessage(userId: string, message: string) {
    this._hubConnection.invoke('SendMessage', userId, message);
  }
  
  private createConnection() {
    const token = localStorage.getItem('token');
    var options = {
      transport: HttpTransportType.WebSockets,
      logging: signalR.LogLevel.Trace,
      accessTokenFactory: function (){ return `Authorization: Bearer ${token}`}
    };

    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl + "?token=" + token)
      .withAutomaticReconnect()
      .build();
  }

  private startConnection(): any {
    return this._hubConnection
      .start()
      .then(() => {
        this.connectionIsEstablished = true;
        console.log('Hub connection started');
        this.connectionId = this._hubConnection.connectionId;
        this.connectionEstablished.emit(true);
      })
      .catch(err => {
        console.log('Error while establishing connection, retrying...');
        setTimeout(function () { this.startConnection(); }, 5000);
      });
  }

  private registerOnServerEvents(): void {
    this._hubConnection.on('UpdateMessage', (data: any) => {
      this.messageReceived.emit(data);
      //this.newLatestMessageReceived.emit(data);
    });
  }

  public disconnected(){
    //this._hubConnection();
  }

  public getListMessage(toUserId: string){
    return this.http.get<Message[]>(this.basaApi + '/listMessages/' + toUserId);
  }
}