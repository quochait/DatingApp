//import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { Component, NgZone, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { User } from '../_models/user';
import { ChatService } from '../_services/chat.service';
import { MessageService } from '../_services/message.service';
import { Message } from './../_models/message';

@Component({
  selector: 'app-messages',
  templateUrl: './message.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  // users: User[];
  user: User;
  userToChild: User;
  messages: Message[];
  latestMessage: Message[];
  model: any;

  constructor(
    private route: ActivatedRoute,
    private messageService: MessageService, 
    private chatService: ChatService,
    private _ngZone: NgZone
  ) {}

  async ngOnInit() {
    this.route.data.subscribe(res => {
      this.model = res.users;
      this.user = res.user;
    });

    if(this.model.length > 0){
      this.userToChild = this.model[0];

      for (let index = 0; index < this.model.length; index++) {
        let objectId = this.model[index]["objectId"];
        let content = 'New message...';
        this.messageService.getLatestMessage(objectId).subscribe(res => {
          this.model[index]["latestMessage"] = res;
          console.log(res.content);
        })
      }
    }
  }

  loadMessageUser(userId: string){
    for (let index = 0; index < this.model.length; index++) {
      if (this.model[index].objectId == userId) {
       console.log(this.model[index]);
       this.userToChild = this.model[index];
      }
    }
  }

  updateMessage(msg){
    for (let index = 0; index < this.model.length; index++) {
      if(this.model[index]["latestMessage"]["groupId"] == msg["groupId"]){
        this.model[index]["latestMessage"] = msg;
      }
    }
  }
}
