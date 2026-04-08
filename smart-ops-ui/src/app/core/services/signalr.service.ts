import { Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject, Observable } from 'rxjs';
import { MetricDto, AlertDto } from '../models/models';
import { TokenStorageService } from './token-storage.service';

@Injectable({ providedIn: 'root' })
export class SignalRService implements OnDestroy {
  private metricConnection: signalR.HubConnection | null = null;
  private alertConnection: signalR.HubConnection | null = null;

  private _metricReceived$ = new Subject<MetricDto>();
  private _alertCreated$ = new Subject<AlertDto>();

  readonly metricReceived$: Observable<MetricDto> = this._metricReceived$.asObservable();
  readonly alertCreated$: Observable<AlertDto> = this._alertCreated$.asObservable();

  constructor(private tokenStorage: TokenStorageService) {}

  startMetricHub(hostId: string): void {
    if (this.metricConnection) return;
    const token = this.tokenStorage.getToken() ?? '';
    this.metricConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/hubs/metrics?access_token=${token}`)
      .withAutomaticReconnect()
      .build();

    this.metricConnection.on('MetricReceived', (data: MetricDto) => {
      this._metricReceived$.next(data);
    });

    this.metricConnection
      .start()
      .then(() => this.metricConnection!.invoke('JoinHostGroup', hostId))
      .catch(console.error);
  }

  startAlertHub(hostId: string): void {
    if (this.alertConnection) return;
    const token = this.tokenStorage.getToken() ?? '';
    this.alertConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/hubs/alerts?access_token=${token}`)
      .withAutomaticReconnect()
      .build();

    this.alertConnection.on('AlertCreated', (data: AlertDto) => {
      this._alertCreated$.next(data);
    });

    this.alertConnection
      .start()
      .then(() => this.alertConnection!.invoke('JoinHostGroup', hostId))
      .catch(console.error);
  }

  stopAll(): void {
    this.metricConnection?.stop();
    this.alertConnection?.stop();
    this.metricConnection = null;
    this.alertConnection = null;
  }

  ngOnDestroy(): void {
    this.stopAll();
    this._metricReceived$.complete();
    this._alertCreated$.complete();
  }
}
