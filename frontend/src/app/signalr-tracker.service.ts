import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

export interface TrackerPositionResponse {
  trackerId: string;
  latitude: number;
  longitude: number;
  timestamp: string;
}

export interface TrackerInfoResponse {
  motorcycle: any;
  trackerId: string;
  userName: string;
  position: TrackerPositionResponse;
}

export interface TrackTrackerListResponse {
  trackName: string;
  trackId: number;
  trackers: TrackerInfoResponse[];
}

@Injectable({ providedIn: 'root' })
export class SignalrTrackerService {
  private hubConnection: signalR.HubConnection | null = null;
  private trackerSubject = new BehaviorSubject<TrackTrackerListResponse | null>(null);
  trackerUpdates$ = this.trackerSubject.asObservable();

  public startConnection(trackId: number) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/your-signalr-hub-url') // URL später anpassen
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveTrackTrackerList', (data: TrackTrackerListResponse) => {
      if (data.trackId === trackId) {
        this.trackerSubject.next(data);
      }
    });

    this.hubConnection.start();
  }

  public stopConnection() {
    this.hubConnection?.stop();
  }
}

