import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { HttpServiceService, Track } from '../http-service.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-track-list',
  standalone: true,
  imports: [CommonModule, MatTableModule],
  templateUrl: './track-list.component.html',
  styleUrls: ['./track-list.component.scss']
})
export class TrackListComponent implements OnInit {
  displayedColumns: string[] = ['id', 'name', 'lengthInKm', 'difficulty'];
  tracks: Track[] = [];

  constructor(private httpService: HttpServiceService, private router: Router) {}

  ngOnInit() {
    this.httpService.getTracks().subscribe(data => {
      this.tracks = data;
    });
  }

  getDifficultyLabel(difficulty: number): string {
    switch (difficulty) {
      case 1: return 'Easy';
      case 2: return 'Medium';
      case 3: return 'Hard';
      case 4: return 'Expert';
      default: return '';
    }
  }

  onRowClick(track: Track) {
    this.router.navigate(['/track-detail', track.id]);
  }
}
