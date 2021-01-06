import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';
import { PhotoService } from 'src/app/_services/photo.service';
import { Photo } from 'src/app/_models/Photo';
import { RelationshipService } from 'src/app/_services/relationship.service';
import { Relationship } from 'src/app/_models/relationship';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  user: User;
  photos: Photo[];
  relationship: Relationship;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  isDisabledMatch: boolean = false;
  isDisabledMessage: boolean = true;
  status: string = "Match";

  constructor(
    private alertify: AlertifyService, 
    private route: ActivatedRoute, 
    private relationshipService: RelationshipService
    ) {}

  ngOnInit() {
    this.route.data.subscribe(res => {
      this.user = res.user;
      this.photos = res.photos;
      this.relationship = res.relationship;
    });

    this.galleryOptions = [
      {
        width: '100%',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        preview: true,
        imageAnimation: NgxGalleryAnimation.Slide
      }
    ];

    this.galleryImages = this.getImages();

    if(this.relationship.status === "Pending"){
      this.buttonMatchToggle();
      // this.buttonMessageToggle();
      this.status = this.relationship.status;
    }
    else if(this.relationship.status === "Matched"){
      this.buttonMatchToggle();
      this.buttonMessageToggle();
      this.status = this.relationship.status;
    }
  
    //console.log(this.buttonMessageToggle);
  }

  getImages() {
    const imageUrls = [];
    
    for (let index = 0; index < this.photos.length; index++) {
      let url = this.photos[index].url;
      imageUrls.push({
        small: url,
        medium: url,
        big: url
      });
    }

    return imageUrls;
  }

  buttonMatchToggle(){
    this.isDisabledMatch = !this.isDisabledMatch;
  }

  buttonMessageToggle(){
    this.isDisabledMessage = !this.isDisabledMessage;
  }

  sendRequestMatch(){
    this.relationshipService.makeRequestMatch(this.user.objectId).subscribe(
      res => {
        this.alertify.success("Waiting for accept.");
        this.buttonMatchToggle();
        this.status = 'Pending';
    }, 
    error => {
      this.alertify.error(error);
    });
  }

  routeToMessage(){

  }
}
