import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { EditProfileComponent } from './updates/edit-profile/edit-profile.component';
import { preventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { memberDetailsResolver } from './_resolvers/member-details.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { adminGuard } from './_guards/admin.guard';
import { preventMessagingGuard } from './_guards/prevent-messaging.guard';
import { LearnMoreComponent } from './learn-more/learn-more.component';
import { RegisterComponent } from './register/register.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      { path: 'members', component: MemberListComponent },
      {
        path: 'members/:username',
        component: MemberDetailsComponent,
        resolve: { member: memberDetailsResolver },
        canActivate: [preventMessagingGuard],
      },
      {
        path: 'member/edit',
        component: EditProfileComponent,
        canDeactivate: [preventUnsavedChangesGuard],
      },
      { path: 'lists', component: ListsComponent },
      { path: 'messages', component: MessagesComponent },
      {
        path: 'admin',
        component: AdminPanelComponent,
        canActivate: [adminGuard],
      },
    ],
  },
  { path: 'learn-more', component: LearnMoreComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'errors', component: TestErrorsComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'server-error', component: ServerErrorComponent },

  { path: '**', component: HomeComponent, pathMatch: 'full' },
];
