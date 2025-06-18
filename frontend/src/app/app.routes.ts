import { Routes } from '@angular/router';
import { UserFormComponent } from './user-form/user-form.component';
import { MotorListComponent } from './motor-list/motor-list.component';
import { TrackListComponent } from './track-list/track-list.component';
import { TrackDetailComponent } from './track-detail/track-detail.component';

export const routes: Routes = [
  { path: '', redirectTo: 'user-form', pathMatch: 'full' },
  { path: 'user-form', component: UserFormComponent },
  { path: 'motor-list', component: MotorListComponent },
  { path: 'track-list', component: TrackListComponent },
  { path: 'track-detail/:id', component: TrackDetailComponent }
];
