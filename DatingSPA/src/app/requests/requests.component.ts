import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, UrlSerializer } from "@angular/router";
import { AlertifyService } from "../_services/alertify.service";
import { PhotoService } from "../_services/photo.service";
import { UserService } from "../_services/user.service";

@Component({
  selector: 'requests',
  templateUrl: './requests.component.html',
  styleUrls: ['./requests.component.css']
})
export class RequestsComponent implements OnInit{
  noRequests: boolean = true;
  isForces: boolean = false;
  requestContent: string = "Chọn bất kì một thông báo."

  public userForces: any;
  public data: any;

  disableBtnAccept: boolean = false;
  disableBtnDeny: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private alertifyService: AlertifyService
  ){}
  
  ngOnInit(){
    this.route.data.subscribe(res => {
      this.data = res;
    });

    let users = this.data["requestsPending"]["users"];
    if (this.data != null)
    {
      this.noRequests = false;
    }
    let relationships = Object.assign({}, this.data["requestsPending"]["relationships"]);
  }

  showRequest(userId){
    this.userService.getUser(userId).subscribe((res) => {
      this.userForces = res;
    });
  }

  updateRelationship(toUserId){
    this.userService.updateRelationship(toUserId).subscribe(() => {
      this.alertifyService.success("Update success relationship.");
      this.disableBtnAccept = true;
      this.disableBtnDeny = true;
    },
    error => {
      this.alertifyService.error("Can't accept relationship.");
    });
  }
}