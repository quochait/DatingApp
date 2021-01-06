import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { RelationshipService } from 'src/app/_services/relationship.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;

  constructor(
    private relationshipService: RelationshipService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    // console.log(this.user);
  }

  sendRequestMatch(userId){
    this.relationshipService.makeRequestMatch(this.user.objectId).subscribe(
      res => {
        this.alertify.success("Waiting for accept.");
    }, 
    error => {
      this.alertify.error(error);
    });
  }

  isHaveRequest(userId){
    this.relationshipService.haveRequest(userId).subscribe((res) => {
      if(res){
        console.log("true");
        return true;
      }
      console.log("False");
      return false;
    })
  }
}
