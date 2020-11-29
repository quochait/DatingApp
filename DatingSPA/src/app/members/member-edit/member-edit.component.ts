import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';
import { Photo } from 'src/app/_models/Photo';
import { NgxGalleryComponent } from 'ngx-gallery';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})

export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  user: User;
  photos: Photo[];
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  urlPhotoToChange: string;
  showFormGetTokenEmail: boolean = true;

  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private userService: UserService,
    private authService: AuthService
  ) {
  }

  ngOnInit() {
    this.route.data.subscribe(res => {
      this.user = res.user;
      console.log(this.user);
      this.photos = res.photos;
    });

    this.galleryOptions = [
      {
        width: '100%',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: true,
        // arrowNextIcon
        thumbnailsPercent: 20,
        thumbnailsMargin: 20,
        thumbnailMargin: 20
      }
    ];

    this.galleryImages = this.getImages();
    this.urlPhotoToChange = this.photos[0].url;

    if(this.user.isVerifyEmail == true)
    {
      this.showFormGetTokenEmail = false;  
    }
    
    console.log(this.showFormGetTokenEmail);
  }

  updateUser() {
    this.userService.updateUser(this.user).subscribe(
      next => {
        this.alertify.success('Profile updated successful!');
        this.editForm.reset(this.user);
      },
      error => {
        this.alertify.error(error);
      }
    );
  }

  updateMainPhoto() {
    this.user.mainPhotoUrl = this.urlPhotoToChange; 
    this.userService.setMainPhoto(this.urlPhotoToChange).subscribe(
      res => {
        this.alertify.success('Update successful');
        this.authService.changeMainPhoto(this.urlPhotoToChange);
      },
      error => {
        this.alertify.error(error);
      }
    );
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

  changeUrlToSetAvatart(event){
    let url = event.image.small;
    this.urlPhotoToChange = url;
  }

  deleteImage(event){

  }

  showConfirmEmail(){
    
  }
}
