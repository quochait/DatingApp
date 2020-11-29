import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { Pipe } from '@angular/core';

import { AppComponent } from './app.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NavComponent } from './nav/nav.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { VerifyEmailComponent } from './members/verify-email/verify-email.component';
import { RegisterComponent } from './home/register/register.component';
import { ErrorInterceptorProvide } from './_services/error.interceptor';
import { AlertifyService } from './_services/alertify.service';
import { AngularFileUploaderModule } from "angular-file-uploader";
import { PhotoListComponent } from './members/member-edit/photo-list/photo-list.component';
import { ResetPasswordComponent } from './home/reset-password/reset-password.component';
import { ChatContentCompoent } from './messages/chat-content/chat-content.component';

// Angular 8
// import {
//   BsDropdownModule,
//   TabsModule,
//   BsDatepickerModule,
//   PaginationModule
// } from 'ngx-bootstrap/modal';

// change to 
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PaginationModule } from 'ngx-bootstrap/pagination';

// import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { RouterModule } from '@angular/router';
import { appRoutes } from './routes';
import { AuthGuard } from './_guards/auth.guard';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberCardComponent } from './members/member-list/member-card/member-card.component';
import { JwtModule } from '@auth0/angular-jwt';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { NgxGalleryModule } from 'ngx-gallery-9';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { FileUploadModule } from 'ng2-file-upload';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TimeAgoPipe } from 'time-ago-pipe';
import { TokenInterceptor } from './_services/token.interceptor';
import { VerifyEmailResolver } from './_resolvers/verify-email.resolver';
import { PhotoUploadResolver } from './_resolvers/photo-upload.resolver';
// import { TimeagoModule } from 'ngx-timeago';
import { NgxImageGalleryModule } from 'ngx-image-gallery';
import { ChatService } from './_services/chat.service';
import { NewPassword } from './home/reset-password/update-password/new-password.component';


// Angular >= 9
// @Pipe({
//     name: 'timeAgoEdit',
//     pure: false
// })
// export class TimeAgoExtendsPipe extends TimeAgoPipe {}


export function tokenGetters() {
  return localStorage.getItem('token');
}

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    ListsComponent,
    MemberListComponent,
    MessagesComponent,
    MemberCardComponent,
    MemberDetailComponent,
    MemberEditComponent,
    PhotoEditorComponent,
    VerifyEmailComponent,
    PhotoListComponent,
    TimeAgoPipe,
    ResetPasswordComponent,
    ChatContentCompoent,
    NewPassword
  ],
  imports: [
    // TooltipModule.forRoot(),
    BrowserModule,
    ReactiveFormsModule,
    HttpClientModule,
    RouterModule.forRoot(appRoutes),
    FormsModule,
    AngularFileUploaderModule,
    // TimeagoModule.forRoot(),
    BsDropdownModule.forRoot(),
    NgxImageGalleryModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetters,
        whitelistedDomains: ['localhost:5001'],
        blacklistedRoutes: ['localhost:5001/api/auth']
      }
    }),
    TabsModule.forRoot(),
    NgxGalleryModule,
    FileUploadModule,
    BsDatepickerModule.forRoot(),
    BrowserAnimationsModule,
    PaginationModule.forRoot()
  ],
  providers: [
    AuthService,
    AlertifyService,
    ErrorInterceptorProvide,
    ChatService,
    AuthGuard,
    MemberDetailResolver,
    MemberListResolver,
    MemberEditResolver,
    PreventUnsavedChangesGuard,
    VerifyEmailResolver,
    PhotoUploadResolver,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    },
    TimeAgoPipe
  ],

  bootstrap: [AppComponent]
})
export class AppModule {}
