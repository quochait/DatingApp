import { Component, OnInit, ViewChild, HostListener, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';
import { Photo } from 'src/app/_models/Photo';

@Component({
  selector: 'app-photo-list',
  templateUrl: './photo-list.component.html'
})

export class PhotoListComponent implements OnInit {
    @Input() photos: Photo[];
    ngOnInit(){}
}