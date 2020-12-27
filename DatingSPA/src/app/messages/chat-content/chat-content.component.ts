import { Component, ElementRef, EventEmitter, HostListener, Injectable, Input, NgZone, Output, ViewChild } from "@angular/core";
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ChatService } from 'src/app/_services/chat.service';
import { MessageService } from 'src/app/_services/message.service';
import { environment } from './../../../environments/environment';


@Component({
  selector: 'app-chat-content',
  templateUrl: './chat-content.component.html',
  styleUrls: ['./chat-content.component.css']
})

export class ChatContentCompoent {
  user: User;
  messages: Message[] = [];
  innerHeight: number;

  contentMessage: string;
  hubUrl: string = environment.hubUrl;

  @ViewChild('mainContents', {read: ElementRef, static:false}) elementView: ElementRef;
  @Input() userToChat: User = null;
  @Output('latestMessage') latestMessageEvent = new EventEmitter<Message>(); 
  @HostListener('window:resize', ['$event'])
  onResize(event){
    this.innerHeight = window.innerHeight - 205;
  }

  constructor(
    private chatService: ChatService,
    private messageService: MessageService,
    private alertify: AlertifyService,
    private _ngZone: NgZone
  ){}

  ngOnInit() {
    this.scrollToBottom();
    
    this.chatService.getListMessage(this.userToChat.objectId).subscribe(msg => {
      this.messages = msg;
    });

    this.chatService.messageReceived.subscribe(res => {
      this._ngZone.run(() => {
        this.messages.push(res);
        this.latestMessageEvent.emit(res);
        
        // Scroll to bottom
        // console.log(this.elementView.nativeElement.scrollHeight);
        // let height = this.elementView.nativeElement.scrollHeight + 1000;
        // console.log(height);
        // this.elementView.nativeElement.scrollTop = height;
        this.scrollToBottom();
      })
    });

    this.innerHeight = window.innerHeight - 205;
  }

  async ngOnDestroy() {
    this.chatService.disconnected();    
  }

  sendMessage(){
    this.chatService.sendMessage(this.userToChat.objectId, this.contentMessage);
    this.contentMessage = '';
  }

  isSent(userId){
    let classList = 'sent'
    if(userId === this.userToChat.objectId){
      classList = 'replies';
    }

    return classList;
  }

  scrollToBottom(): void {
    try {
        console.log(this.elementView.nativeElement.scrollHeight);
        this.elementView.nativeElement.scrollTop = this.elementView.nativeElement.scrollHeight - 1;
    } catch(err) { }                 
  }
}