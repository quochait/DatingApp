import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';
import { PhotoService } from 'src/app/_services/photo.service';
import { Photo } from 'src/app/_models/Photo';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  
  constructor(
    private userService: UserService, 
    private alertify: AlertifyService, 
    private route: ActivatedRoute, 
    private photoService: PhotoService) {}

  ngOnInit() {
    this.route.data.subscribe(res => {
      this.user = res.user;
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

    this.galleryImages = this.getImages();
  }

  getImages() {
    const imageUrls = [];
    // tslint:disable-next-line: prefer-for-of
    let photos = JSON.stringify(this.photoService.getPhotos())

    // for (let index = 0; index < photos.length; index++) {
    //   imageUrls.push({
    //     small: photos.,
    //     medium: photos[index].url,
    //     big: photos[index].url
    //   });
    // }
    
    console.log(photos);

    return imageUrls;
  }
}
