import { Component, ElementRef, EventEmitter, HostListener, Injectable, Input, NgZone, Output, ViewChild } from "@angular/core";
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ChatService } from 'src/app/_services/chat.service';
import { MessageService } from 'src/app/_services/message.service';
import { environment } from './../../../environments/environment';
import { PickerModule } from '@ctrl/ngx-emoji-mart'
import * as $ from 'jquery';

@Component({
  selector: 'app-chat-content',
  templateUrl: './chat-content.component.html',
  styleUrls: ['./chat-content.component.css']
})

export class ChatContentCompoent {
  user: User;
  messages: Message[] = [];
  innerHeight: number;
  contentMessage: string = '';
  hubUrl: string = environment.hubUrl;
  showEmojiPicker = false;

  @ViewChild('mainContents', {read: ElementRef, static:false}) elementView: ElementRef;
  @Input() userToChat: User = null;
  @Output('latestMessage') latestMessageEvent = new EventEmitter<Message>(); 
  @HostListener('window:resize', ['$event'])
  onResize(event){
    this.innerHeight = window.innerHeight - 205;
  }

  height: number;

  constructor(
    private chatService: ChatService,
    private _ngZone: NgZone
  ){}

  ngOnInit() {
    
    this.chatService.getListMessage(this.userToChat.objectId).subscribe(msg => {
      this.messages = msg;
      this.height = $("#mainContents").prop("scrollHeight");
      $("#mainContents").scrollTop($("#mainContents").prop("scrollHeight"));
    });

    this.chatService.messageReceived.subscribe(res => {
      console.log($("#mainContents").prop("scrollHeight"))
      this._ngZone.run(() => {
        this.messages.push(res);
        this.latestMessageEvent.emit(res);
      });

      this._ngZone.runOutsideAngular(() => {
        console.log($("#mainContents").prop("scrollHeight"))
      })
     
      $("#mainContents").scrollTop($("#mainContents").prop("scrollHeight"));
    });

    this.innerHeight = window.innerHeight - 205;
  }

  ngOnChanges(): void {
    this.chatService.getListMessage(this.userToChat.objectId).subscribe(msg => {
      this.messages = msg;
    });
  }

  ngAfterViewInit(): void {
    import('./extension.js');
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

  scrollToBottom(index) {
    $("#mainContents").scrollTop($("#mainContents").prop("scrollHeight"));
    $("#mainContents").scrollTop(100000);

  }

  toggleEmojiPicker(){
    this.showEmojiPicker = !this.showEmojiPicker;
    // console.log("[>] click.")
  }

  addEmoji(event){
    const text = event.emoji.native;
    this.contentMessage += " " + text;
    this.showEmojiPicker = !this.showEmojiPicker; 
  }
}