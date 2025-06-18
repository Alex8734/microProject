import { Component } from '@angular/core';
import {RouterLink, RouterOutlet} from '@angular/router';
import { UserFormComponent } from './user-form/user-form.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MotorListComponent } from './motor-list/motor-list.component';
import { Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { TrackListComponent } from './track-list/track-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, UserFormComponent, MatToolbarModule, MatButtonModule, MotorListComponent, RouterLink, HttpClientModule, TrackListComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'frontend';
  constructor(private router: Router) {}

  navigateTo(route: string) {
    this.router.navigate(['/' + route]);
  }
}
