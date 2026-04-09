import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of, Subject, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { AlertsComponent } from './alerts.component';
import { AlertService } from '../../core/services/alert.service';
import { HostService } from '../../core/services/host.service';
import { SignalRService } from '../../core/services/signalr.service';
import { AlertSeverity, AlertStatus } from '../../core/models/models';
import { AlertSeverityPipe, AlertStatusPipe } from '../../shared/pipes/status.pipe';

const mockAlert = {
  id: 'a1', hostId: 'h1', serviceNodeId: null, title: 'T', message: 'M',
  severity: AlertSeverity.Critical, status: AlertStatus.Open,
  acknowledgedAt: null, resolvedAt: null, acknowledgedByUserId: null, createdAt: '',
};

const mockHost = {
  id: 'h1', name: 'Host1', ipAddress: '10.0.0.1', osType: 'Linux',
  status: 1, tags: [], createdAt: '', updatedAt: '',
};

describe('AlertsComponent', () => {
  let component: AlertsComponent;
  let fixture: ComponentFixture<AlertsComponent>;
  let alertService: jasmine.SpyObj<AlertService>;
  let hostService: jasmine.SpyObj<HostService>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;
  let alertSubject$: Subject<any>;

  beforeEach(async () => {
    alertSubject$ = new Subject();
    alertService = jasmine.createSpyObj('AlertService', ['getAll', 'updateStatus', 'create']);
    hostService = jasmine.createSpyObj('HostService', ['getAll']);
    snackBar = jasmine.createSpyObj('MatSnackBar', ['open']);

    alertService.getAll.and.returnValue(of([mockAlert]));
    hostService.getAll.and.returnValue(of([mockHost]));

    const signalRService = { alertCreated$: alertSubject$.asObservable() };

    await TestBed.configureTestingModule({
      declarations: [AlertsComponent, AlertSeverityPipe, AlertStatusPipe],
      providers: [
        { provide: AlertService, useValue: alertService },
        { provide: HostService, useValue: hostService },
        { provide: SignalRService, useValue: signalRService },
        { provide: MatSnackBar, useValue: snackBar },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(AlertsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => component.ngOnDestroy());

  it('should create', () => expect(component).toBeTruthy());

  it('loads alerts and hosts on init', () => {
    expect(component.alerts).toEqual([mockAlert]);
    expect(component.hosts).toEqual([mockHost]);
    expect(component.loading).toBeFalse();
  });

  it('loadAlerts with filters', () => {
    component.filterStatus = 'Open';
    component.filterSeverity = 'Critical';
    component.filterHostId = 'h1';
    component.loadAlerts();
    expect(alertService.getAll).toHaveBeenCalledWith({ status: 'Open', severity: 'Critical', hostId: 'h1' });
  });

  it('handles error on loadAlerts', () => {
    alertService.getAll.and.returnValue(throwError(() => new Error('err')));
    component.loadAlerts();
    expect(component.loading).toBeFalse();
  });

  it('acknowledge calls updateStatus', () => {
    const updated = { ...mockAlert, status: AlertStatus.Acknowledged };
    alertService.updateStatus.and.returnValue(of(updated));
    component.acknowledge(mockAlert);
    expect(alertService.updateStatus).toHaveBeenCalledWith('a1', { status: AlertStatus.Acknowledged });
    expect(snackBar.open).toHaveBeenCalled();
  });

  it('resolve calls updateStatus', () => {
    const updated = { ...mockAlert, status: AlertStatus.Resolved };
    alertService.updateStatus.and.returnValue(of(updated));
    component.resolve(mockAlert);
    expect(alertService.updateStatus).toHaveBeenCalledWith('a1', { status: AlertStatus.Resolved });
    expect(snackBar.open).toHaveBeenCalled();
  });

  it('createAlert does nothing when form invalid', () => {
    component.createAlert();
    expect(alertService.create).not.toHaveBeenCalled();
  });

  it('createAlert creates alert on valid form', () => {
    alertService.create.and.returnValue(of(mockAlert));
    component.createForm.setValue({ hostId: 'h1', title: 'T', message: 'M', severity: AlertSeverity.Info });
    component.createAlert();
    expect(alertService.create).toHaveBeenCalled();
    expect(snackBar.open).toHaveBeenCalled();
  });

  it('hostName returns host name for known host', () => {
    expect(component.hostName('h1')).toBe('Host1');
  });

  it('hostName returns truncated id for unknown host', () => {
    expect(component.hostName('unknown-host-id')).toBe('unknown-');
  });

  it('handles incoming SignalR alert', () => {
    const before = component.alerts.length;
    alertSubject$.next(mockAlert);
    expect(component.alerts.length).toBe(before + 1);
  });

  it('ngOnDestroy completes destroy$', () => {
    expect(() => component.ngOnDestroy()).not.toThrow();
  });
});
