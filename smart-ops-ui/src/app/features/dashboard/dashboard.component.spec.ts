import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { DashboardComponent } from './dashboard.component';
import { HostService } from '../../core/services/host.service';
import { AlertService } from '../../core/services/alert.service';
import { MetricService } from '../../core/services/metric.service';
import { SignalRService } from '../../core/services/signalr.service';
import { HostStatus, AlertSeverity, AlertStatus } from '../../core/models/models';
import { HostStatusPipe, AlertSeverityPipe, AlertStatusPipe } from '../../shared/pipes/status.pipe';

const mockHosts = [
  { id: 'h1', name: 'Host1', ipAddress: '10.0.0.1', osType: 'Linux', status: HostStatus.Online, tags: [], createdAt: '', updatedAt: '' },
  { id: 'h2', name: 'Host2', ipAddress: '10.0.0.2', osType: 'Linux', status: HostStatus.Offline, tags: [], createdAt: '', updatedAt: '' },
];

const mockAlerts = [
  { id: 'a1', hostId: 'h1', serviceNodeId: null, title: 'T1', message: 'M', severity: AlertSeverity.Critical, status: AlertStatus.Open, acknowledgedAt: null, resolvedAt: null, acknowledgedByUserId: null, createdAt: '2024-01-01T00:00:00Z' },
  { id: 'a2', hostId: 'h1', serviceNodeId: null, title: 'T2', message: 'M', severity: AlertSeverity.Warning, status: AlertStatus.Open, acknowledgedAt: null, resolvedAt: null, acknowledgedByUserId: null, createdAt: '2024-01-01T00:00:00Z' },
];

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let hostService: jasmine.SpyObj<HostService>;
  let alertService: jasmine.SpyObj<AlertService>;
  let metricService: jasmine.SpyObj<MetricService>;
  let metricSubject$: Subject<any>;
  let alertSubject$: Subject<any>;

  beforeEach(async () => {
    metricSubject$ = new Subject();
    alertSubject$ = new Subject();

    hostService = jasmine.createSpyObj('HostService', ['getAll']);
    alertService = jasmine.createSpyObj('AlertService', ['getAll']);
    metricService = jasmine.createSpyObj('MetricService', ['getByHost']);

    hostService.getAll.and.returnValue(of(mockHosts));
    alertService.getAll.and.returnValue(of(mockAlerts));
    metricService.getByHost.and.returnValue(of([]));

    const signalRService = {
      metricReceived$: metricSubject$.asObservable(),
      alertCreated$: alertSubject$.asObservable(),
      startMetricHub: jasmine.createSpy(),
      startAlertHub: jasmine.createSpy(),
      stopAll: jasmine.createSpy(),
    };

    await TestBed.configureTestingModule({
      declarations: [DashboardComponent, HostStatusPipe, AlertSeverityPipe, AlertStatusPipe],
      providers: [
        { provide: HostService, useValue: hostService },
        { provide: AlertService, useValue: alertService },
        { provide: MetricService, useValue: metricService },
        { provide: SignalRService, useValue: signalRService },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    component.ngOnDestroy();
  });

  it('should create', () => expect(component).toBeTruthy());

  it('loads hosts and alerts on init', () => {
    expect(component.hosts).toEqual(mockHosts);
    expect(component.openAlerts.length).toBe(2);
    expect(component.loading).toBeFalse();
  });

  it('computes summary counts', () => {
    expect(component.onlineCount).toBe(1);
    expect(component.offlineCount).toBe(1);
    expect(component.openAlertCount).toBe(2);
    expect(component.criticalCount).toBe(1);
  });

  it('builds initial chart options', () => {
    expect(component.cpuGaugeOption).toBeTruthy();
    expect(component.memGaugeOption).toBeTruthy();
    expect(component.alertPieOption).toBeTruthy();
  });

  it('updates gauges on live cpu metric', () => {
    const metric = { id: 'm1', hostId: 'h1', serviceNodeId: null, metricType: 'cpu', value: 75, unit: '%', timestamp: '', labels: {} };
    metricSubject$.next(metric);
    expect(component.cpuGaugeOption).toBeTruthy();
  });

  it('updates gauges on live mem metric', () => {
    const metric = { id: 'm2', hostId: 'h1', serviceNodeId: null, metricType: 'memory', value: 60, unit: '%', timestamp: '', labels: {} };
    metricSubject$.next(metric);
    expect(component.memGaugeOption).toBeTruthy();
  });

  it('handles incoming SignalR alert', () => {
    const before = component.openAlerts.length;
    alertSubject$.next(mockAlerts[0]);
    expect(component.openAlerts.length).toBe(before + 1);
  });

  it('ngOnDestroy completes destroy$', () => {
    expect(() => component.ngOnDestroy()).not.toThrow();
  });
});
