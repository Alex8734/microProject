import { Component, ViewChild, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SignalrTrackerService, TrackTrackerListResponse, TrackerInfoResponse } from '../signalr-tracker.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-track-detail',
  standalone: true,
  imports: [],
  templateUrl: './track-detail.component.html',
  styleUrl: './track-detail.component.scss'
})
export class TrackDetailComponent implements AfterViewInit, OnDestroy {
  @ViewChild('trackCanvas', { static: false }) canvasRef!: ElementRef<HTMLCanvasElement>;
  trackId: number = 1;
  imagePath: string = '';
  trackerData: TrackTrackerListResponse | null = null;
  private trackerSub?: Subscription;
  private img: HTMLImageElement = new window.Image();

  constructor(private route: ActivatedRoute, private trackerService: SignalrTrackerService) {
    this.route.params.subscribe(params => {
      this.trackId = +params['id'] || 1;
      this.imagePath = `/image/track${this.trackId}.png`;
      this.trackerService.startConnection(this.trackId);
    });
  }

  ngAfterViewInit() {
    this.img.onload = () => this.drawCanvas();
    this.img.src = this.imagePath;
    this.trackerSub = this.trackerService.trackerUpdates$.subscribe(data => {
      if (data && data.trackId === this.trackId) {
        this.trackerData = data;
        this.drawCanvas();
      }
    });
  }

  ngOnDestroy() {
    this.trackerSub?.unsubscribe();
    this.trackerService.stopConnection();
  }

  private drawCanvas() {
    const canvas = this.canvasRef.nativeElement;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.drawImage(this.img, 0, 0, canvas.width, canvas.height);
    if (this.trackerData) {
      this.trackerData.trackers.forEach((tracker, i) => {
        this.drawTracker(ctx, tracker, i);
      });
    }
  }

  private drawTracker(ctx: CanvasRenderingContext2D, tracker: TrackerInfoResponse, idx: number) {
    // Dummy-Umrechnung: Longitude/Latitude auf Canvas (hier einfach skaliert)
    const x = 50 + (tracker.position.longitude * 400) % 500;
    const y = 50 + (tracker.position.latitude * 300) % 350;
    const colors = ['#ff6600', '#4caf50', '#2196f3', '#e53935', '#ffd600'];
    ctx.beginPath();
    ctx.arc(x, y, 12, 0, 2 * Math.PI);
    ctx.fillStyle = colors[idx % colors.length];
    ctx.globalAlpha = 0.85;
    ctx.fill();
    ctx.globalAlpha = 1;
    ctx.font = 'bold 16px Oswald, Arial';
    ctx.fillStyle = '#181818';
    ctx.fillText(tracker.userName, x + 16, y + 6);
  }
}
