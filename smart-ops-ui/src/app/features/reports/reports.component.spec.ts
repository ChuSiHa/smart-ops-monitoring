import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { ReportsComponent } from './reports.component';
import { HostService } from '../../core/services/host.service';
import { AlertService } from '../../core/services/alert.service';
import { MetricService } from '../../core/services/metric.service';
import { HostStatus, AlertSeverity, AlertStatus } from '../../core/models/models';

const mockHosts = [
  { id: 'h1', name: 'Host1', ipAddress: '', osType: '', status: HostStatus.Online, tags: [], createdAt: '', updatedAt: '' },
  { id: 'h2', name: 'Host2', ipAddress: '', osType: '', status: HostStatus.Offline, tags: [], createdAt: '', updatedAt: '' },
];

const today = new Date().toISOString();
const mockAlerts = [
  { id: 'a1', hostId: 'h1', serviceNodeId: null, title: 'T', message: 'M', severity: AlertSeverity.Critical, status: AlertStatus.Open, acknowledgedAt: null, resolvedAt: null, acknowledgedByUserId: null, createdAt: today },
  { id: 'a2', hostId: 'h1', serviceNodeId: null, title: 'T', message: 'M', severity: AlertSeverity.Warning, status: AlertStatus.Open, acknowledgedAt: null, resolvedAt: null, acknowledgedByUserId: null, createdAt: today },
  { id: 'a3', hostId: 'h1', serviceNodeId: null, title: 'T', message: 'M', severity: AlertSeverity.Info, status: AlertStatus.Open, acknowledgedAt: null, resolvedAt: null, acknowledgedByUserId: null, createdAt: today },
];

describe('ReportsComponent', () => {
  let component: ReportsComponent;
  let fixture: ComponentFixture<ReportsComponent>;
  let hostService: jasmine.SpyObj<HostService>;
  let alertService: jasmine.SpyObj<AlertService>;
  let metricService: jasmine.SpyObj<MetricService>;

  beforeEach(async () => {
    hostService = jasmine.createSpyObj('HostService', ['getAll']);
    alertService = jasmine.createSpyObj('AlertService', ['getAll']);
    metricService = jasmine.createSpyObj('MetricService', ['getByHost']);

    hostService.getAll.and.returnValue(of(mockHosts));
    alertService.getAll.and.returnValue(of(mockAlerts));
    metricService.getByHost.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      declarations: [ReportsComponent],
      providers: [
        { provide: HostService, useValue: hostService },
        { provide: AlertService, useValue: alertService },
        { provide: MetricService, useValue: metricService },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(ReportsComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => expect(component).toBeTruthy());

  it('loads data and builds charts on init', fakeAsync(() => {
    fixture.detectChanges();
    tick();
    expect(component.loading).toBeFalse();
    expect(component.hostPieOption).toBeTruthy();
    expect(component.alertBarOption).toBeTruthy();
    expect(component.heatmapOption).toBeTruthy();
  }));

  it('works with empty data', fakeAsync(() => {
    hostService.getAll.and.returnValue(of([]));
    alertService.getAll.and.returnValue(of([]));
    fixture.detectChanges();
    tick();
    expect(component.loading).toBeFalse();
  }));
});
