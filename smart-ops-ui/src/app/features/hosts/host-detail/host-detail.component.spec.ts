import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { HostDetailComponent } from './host-detail.component';
import { HostService } from '../../../core/services/host.service';
import { ServiceNodeService } from '../../../core/services/service-node.service';
import { HostStatus, ServiceNodeStatus } from '../../../core/models/models';
import { HostStatusPipe, ServiceNodeStatusPipe } from '../../../shared/pipes/status.pipe';

const mockHost = {
  id: 'h1', name: 'Host1', ipAddress: '10.0.0.1', osType: 'Linux',
  status: HostStatus.Online, tags: [], createdAt: '', updatedAt: '',
};

const mockNode = {
  id: 'n1', name: 'Svc1', type: 'web', hostId: 'h1',
  status: ServiceNodeStatus.Running, port: 8080, createdAt: '', updatedAt: '',
};

describe('HostDetailComponent', () => {
  let component: HostDetailComponent;
  let fixture: ComponentFixture<HostDetailComponent>;
  let hostService: jasmine.SpyObj<HostService>;
  let serviceNodeService: jasmine.SpyObj<ServiceNodeService>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    hostService = jasmine.createSpyObj('HostService', ['getById']);
    serviceNodeService = jasmine.createSpyObj('ServiceNodeService', ['getByHost', 'create']);
    snackBar = jasmine.createSpyObj('MatSnackBar', ['open']);

    hostService.getById.and.returnValue(of(mockHost));
    serviceNodeService.getByHost.and.returnValue(of([mockNode]));

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [HostDetailComponent, HostStatusPipe, ServiceNodeStatusPipe],
      providers: [
        { provide: HostService, useValue: hostService },
        { provide: ServiceNodeService, useValue: serviceNodeService },
        { provide: MatSnackBar, useValue: snackBar },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => 'h1' } } } },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(HostDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => expect(component).toBeTruthy());

  it('loads host and service nodes on init', () => {
    expect(component.host).toEqual(mockHost);
    expect(component.serviceNodes).toEqual([mockNode]);
    expect(component.loading).toBeFalse();
  });

  it('handles error on init', () => {
    hostService.getById.and.returnValue(throwError(() => new Error('err')));
    serviceNodeService.getByHost.and.returnValue(of([]));
    component.ngOnInit();
    expect(component.loading).toBeFalse();
  });

  it('addNode does nothing when form invalid', () => {
    component.addNode();
    expect(serviceNodeService.create).not.toHaveBeenCalled();
  });

  it('addNode does nothing when no host', () => {
    component.host = null;
    component.nodeForm.setValue({ name: 'n', type: 't', port: null });
    component.addNode();
    expect(serviceNodeService.create).not.toHaveBeenCalled();
  });

  it('addNode creates service node on valid form', () => {
    serviceNodeService.create.and.returnValue(of(mockNode));
    component.nodeForm.setValue({ name: 'Svc', type: 'db', port: 5432 });
    component.addNode();
    expect(serviceNodeService.create).toHaveBeenCalledWith({ name: 'Svc', type: 'db', hostId: 'h1', port: 5432 });
    expect(snackBar.open).toHaveBeenCalled();
    expect(component.showAddNode).toBeFalse();
  });

  it('addNode handles null port', () => {
    serviceNodeService.create.and.returnValue(of(mockNode));
    component.nodeForm.setValue({ name: 'Svc', type: 'db', port: null });
    component.addNode();
    expect(serviceNodeService.create).toHaveBeenCalledWith(jasmine.objectContaining({ port: null }));
  });
});
