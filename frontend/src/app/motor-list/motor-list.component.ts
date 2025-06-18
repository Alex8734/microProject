import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { HttpServiceService, Motorcycle } from '../http-service.service';
import {MatToolbar} from '@angular/material/toolbar';
import {MatButton} from '@angular/material/button';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-motor-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatToolbar, MatButton, RouterLink],
  templateUrl: './motor-list.component.html',
  styleUrl: './motor-list.component.scss'
})
export class MotorListComponent implements OnInit {
  displayedColumns: string[] = ['id', 'model', 'number', 'horsepower', 'isRented'];
  motorcycles: Motorcycle[] = [];

  constructor(private httpService: HttpServiceService) {}

  ngOnInit() {
    this.httpService.getMotorcycles().subscribe(data => {
      this.motorcycles = data;
    });
  }
}
