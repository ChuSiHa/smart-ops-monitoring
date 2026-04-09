import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of, Subject, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { MetricsComponent } from './metrics.component';
import { MetricService } from '../../core/services/metric.service';
import { HostService } from '../../core/services/host.service';
import { SignalRService } from '../../core/services/signalr.service';
import { HostStatus } from '../../core/models/models';

const mockHosts = [
  { id: 'h1', name: 'Host1', ipAddress: '10.0.0.1', osType: 'Linux', status: HostStatus.Online, tags: [], createdAt: '', updatedAt: '' },
];

const mockMetric = { id: 'm1', hostId: 'h1', serviceNodeId: null, metricType: 'cpu', value: 50, unit: '%', timestamp: '2024-01-01T00:00:00Z', labels: {} };

describe('MetricsComponent', () => {
  let component: MetricsComponent;
  let fixture: ComponentFixture<MetricsComponent>;
  let metricService: jasmine.SpyObj<MetricService>;
  let hostService: jasmine.SpyObj<HostService>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;
  let metricSubject$: Subject<any>;

  beforeEach(async () => {
    metricSubject$ = new Subject();
    metricService = jasmine.createSpyObj('MetricService', ['getByHost', 'ingest']);
    hostService = jasmine.createSpyObj('HostService', ['getAll']);
    snackBar = jasmine.createSpyObj('MatSnackBar', ['open']);

    hostService.getAll.and.returnValue(of(mockHosts));
    metricService.getByHost.and.returnValue(of([mockMetric]));

    const signalRService = {
      metricReceived$: metricSubject$.asObservable(),
      startMetricHub: jasmine.createSpy(),
    };

    await TestBed.configureTestingModule({
      declarations: [MetricsComponent],
      providers: [
        { provide: MetricService, useValue: metricService },
        { provide: HostService, useValue: hostService },
        { provide: SignalRService, useValue: signalRService },
        { provide: MatSnackBar, useValue: snackBar },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(MetricsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => expect(component).toBeTruthy());

  it('loads hosts and metrics on init', () => {
    expect(component.hosts).toEqual(mockHosts);
    expect(component.selectedHostId).toBe('h1');
    expect(component.metrics).toEqual([mockMetric]);
  });

  it('loadMetrics does nothing without selectedHostId', () => {
    component.selectedHostId = '';
    component.loadMetrics();
    expect(metricService.getByHost).toHaveBeenCalledTimes(1); // only from ngOnInit
  });

  it('loadMetrics fetches metrics with filter', () => {
    component.selectedHostId = 'h1';
    component.filterType = 'cpu';
    component.loadMetrics();
    expect(metricService.getByHost).toHaveBeenCalledWith('h1', { metricType: 'cpu' });
  });

  it('handles error on loadMetrics', () => {
    metricService.getByHost.and.returnValue(throwError(() => new Error('err')));
    component.loadMetrics();
    expect(component.loading).toBeFalse();
  });

  it('onHostChange calls loadMetrics', () => {
    spyOn(component, 'loadMetrics');
    component.onHostChange();
    expect(component.loadMetrics).toHaveBeenCalled();
  });

  it('ingest does nothing when form invalid', () => {
    component.ingest();
    expect(metricService.ingest).not.toHaveBeenCalled();
  });

  it('ingest sends metric and reloads when same host', () => {
    metricService.ingest.and.returnValue(of(mockMetric));
    component.ingestForm.setValue({ hostId: 'h1', metricType: 'cpu', value: 50, unit: '%' });
    component.selectedHostId = 'h1';
    component.ingest();
    expect(metricService.ingest).toHaveBeenCalled();
    expect(snackBar.open).toHaveBeenCalled();
  });

  it('ingest does not reload when different host', () => {
    metricService.ingest.and.returnValue(of(mockMetric));
    component.ingestForm.setValue({ hostId: 'h2', metricType: 'cpu', value: 50, unit: '%' });
    component.selectedHostId = 'h1';
    const callCount = metricService.getByHost.calls.count();
    component.ingest();
    expect(metricService.getByHost.calls.count()).toBe(callCount);
  });

  it('appends live metric from SignalR for selected host', () => {
    const before = component.metrics.length;
    metricSubject$.next({ ...mockMetric, id: 'm2' });
    expect(component.metrics.length).toBe(before + 1);
  });

  it('ignores live metric from different host', () => {
    const before = component.metrics.length;
    metricSubject$.next({ ...mockMetric, id: 'm3', hostId: 'OTHER' });
    expect(component.metrics.length).toBe(before);
  });

  it('builds chart options', () => {
    expect(component.lineChartOption).toBeTruthy();
  });
});
