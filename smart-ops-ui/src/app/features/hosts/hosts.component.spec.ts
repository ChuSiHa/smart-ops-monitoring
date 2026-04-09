import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { HostsComponent } from './hosts.component';
import { HostService } from '../../core/services/host.service';
import { HostStatus } from '../../core/models/models';
import { HostStatusPipe } from '../../shared/pipes/status.pipe';

const mockHost = {
  id: '1', name: 'Host1', ipAddress: '10.0.0.1', osType: 'Linux',
  status: HostStatus.Online, tags: [], createdAt: '', updatedAt: '',
};

describe('HostsComponent', () => {
  let component: HostsComponent;
  let fixture: ComponentFixture<HostsComponent>;
  let hostService: jasmine.SpyObj<HostService>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    hostService = jasmine.createSpyObj('HostService', ['getAll', 'create']);
    snackBar = jasmine.createSpyObj('MatSnackBar', ['open']);
    hostService.getAll.and.returnValue(of([mockHost]));

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [HostsComponent, HostStatusPipe],
      providers: [
        { provide: HostService, useValue: hostService },
        { provide: MatSnackBar, useValue: snackBar },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(HostsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => expect(component).toBeTruthy());

  it('loads hosts on init', () => {
    expect(hostService.getAll).toHaveBeenCalled();
    expect(component.hosts).toEqual([mockHost]);
    expect(component.loading).toBeFalse();
  });

  it('handles error on loadHosts', () => {
    hostService.getAll.and.returnValue(throwError(() => new Error('err')));
    component.loadHosts();
    expect(component.loading).toBeFalse();
  });

  it('createHost does nothing when form invalid', () => {
    component.createHost();
    expect(hostService.create).not.toHaveBeenCalled();
  });

  it('createHost calls service and updates list on success', () => {
    const newHost = { ...mockHost, id: '2', name: 'Host2' };
    hostService.create.and.returnValue(of(newHost));
    component.createForm.setValue({ name: 'Host2', ipAddress: '10.0.0.2', osType: 'Linux', tags: 'web,db' });
    component.createHost();
    expect(hostService.create).toHaveBeenCalledWith({ name: 'Host2', ipAddress: '10.0.0.2', osType: 'Linux', tags: ['web', 'db'] });
    expect(component.hosts[0]).toEqual(newHost);
    expect(component.showCreateForm).toBeFalse();
    expect(snackBar.open).toHaveBeenCalled();
  });

  it('createHost handles empty tags', () => {
    const newHost = { ...mockHost, id: '3' };
    hostService.create.and.returnValue(of(newHost));
    component.createForm.setValue({ name: 'H', ipAddress: '1.1.1.1', osType: 'Win', tags: '' });
    component.createHost();
    expect(hostService.create).toHaveBeenCalledWith(jasmine.objectContaining({ tags: [] }));
  });
});
