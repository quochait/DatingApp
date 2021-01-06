import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { PhotoUploadResolver } from './_resolvers/photo-upload.resolver';
import { VerifyEmailComponent } from './members/verify-email/verify-email.component';
import { VerifyEmailResolver } from './_resolvers/verify-email.resolver';
import { PhotosDetailResolver } from './_resolvers/photos-details.resolver';
import { RelationshipResolver } from './_resolvers/relationship.resolver';
import { MessageResolver } from './_resolvers/message.resolver';
import { ChatContentCompoent } from './messages/chat-content/chat-content.component';
import { LatestMessageResolver } from './_resolvers/latest-message.resolver';
import { NewPassword } from './home/reset-password/update-password/new-password.component';
import { RequestsComponent } from './requests/requests.component';
import { RequestsResolver } from './_resolvers/requests.resolver';


export const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: 'resetPassword/:token',
    component: NewPassword
  },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'members', component: MemberListComponent, 
        resolve: 
        { 
          users: MemberListResolver 
        } 
      },
      {
        path: 'member/edit',
        component: MemberEditComponent,
        resolve: { user: MemberEditResolver, photos: PhotoUploadResolver },
        canDeactivate: [PreventUnsavedChangesGuard]
      },
      {
        path: 'member/verifyEmail/:token',
        component: VerifyEmailComponent,
        resolve: { token: VerifyEmailResolver, user: MemberEditResolver }
      },
      {
        path: 'member/:id', component: MemberDetailComponent,
        resolve: { 
          user: MemberDetailResolver,
          photos: PhotosDetailResolver, 
          relationship: RelationshipResolver 
        }
      },
      {
        path: 'messages', component: MessagesComponent,
        resolve: {
          user: MemberEditResolver, 
          users: MessageResolver,
        }
      },
      {
        path: 'requests',
        component: RequestsComponent,
        resolve: { requestsPending: RequestsResolver}
      },
      { path: 'lists', component: ListsComponent },
    ]
  },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];
