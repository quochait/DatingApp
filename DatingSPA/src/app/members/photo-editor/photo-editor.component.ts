import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/Photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from '../../../environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  uploader: FileUploader;
  currentMain: Photo;
  hasBaseDropZoneOver = false;
  hasAnotherDropZoneOver = false;
  baseUrl = environment.apiUrl;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.uploader = new FileUploader({
      url:
        this.baseUrl +
        'user/' +
        this.authService.decodeToken.nameid +
        '/photos',
      authToken: 'Bearer ' + localStorage.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = file => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };

        this.photos.push(photo);
        if (photo.isMain) {
          this.authService.changeMainPhoto(photo.url);
        }
      }
    };
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  fileOverAnother(e: any): void {
    this.hasAnotherDropZoneOver = e;
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(photo.id).subscribe(
      res => {
        this.alertify.success('Update successful');
        this.currentMain = this.photos.filter(p => p.isMain === true)[0];
        this.currentMain.isMain = false;
        photo.isMain = true;
        this.authService.changeMainPhoto(photo.url);
      },
      error => {
        this.alertify.error(error);
      }
    );
  }

  deletePhoto(id: number) {
    this.userService.deletePhoto(id).subscribe(
      res => {
        const index = this.photos.findIndex(p => p.id === id);
        this.photos.splice(index, 1);
        this.alertify.success('Detele Photo successful');
      },
      err => {
        this.alertify.error('Failed to delete photo.');
      }
    );
  }
}
