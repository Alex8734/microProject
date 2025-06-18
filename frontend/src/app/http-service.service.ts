import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Motorcycle {
  id: number;
  model: string;
  number: string;
  horsepower: number;
  isRented: boolean;
}

export interface User {
  ssn: number;
  name: string;
  age: number;
  weight: number;
}

export interface Track {
  id: number;
  name: string;
  lengthInKm: number;
  difficulty: number; // 1=Easy, 2=Medium, 3=Hard, 4=Expert
}

@Injectable({
  providedIn: 'root'
})
export class HttpServiceService {
  constructor(private http: HttpClient) { }

  getMotorcycles(): Observable<Motorcycle[]> {
    // Motocross Dummy-Daten
    return new Observable(observer => {
      observer.next([
        { id: 1, model: 'KTM SX-F 450', number: 'MX-101', horsepower: 63, isRented: false },
        { id: 2, model: 'Yamaha YZ250F', number: 'MX-202', horsepower: 40, isRented: true },
        { id: 3, model: 'Honda CRF450R', number: 'MX-303', horsepower: 55, isRented: false }
      ]);
      observer.complete();
    });
  }

  getUsers(): Observable<User[]> {
    // Motocross Dummy-User
    return new Observable(observer => {
      observer.next([
        { ssn: 111111, name: 'Tom Racer', age: 22, weight: 75 },
        { ssn: 222222, name: 'Lena Speed', age: 28, weight: 62 }
      ]);
      observer.complete();
    });
  }

  getTracks(): Observable<Track[]> {
    // Motocross Dummy-Strecken
    return new Observable(observer => {
      observer.next([
        { id: 1, name: 'Sand Valley', lengthInKm: 1.8, difficulty: 2 },
        { id: 2, name: 'Forest Run', lengthInKm: 2.5, difficulty: 3 },
        { id: 3, name: 'Hill Climb', lengthInKm: 1.2, difficulty: 4 }
      ]);
      observer.complete();
    });
  }
}
