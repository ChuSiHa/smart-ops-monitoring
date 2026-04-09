import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { SettingsComponent } from './settings.component';

describe('SettingsComponent', () => {
  let component: SettingsComponent;
  let fixture: ComponentFixture<SettingsComponent>;
  let snackBar: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    snackBar = jasmine.createSpyObj('MatSnackBar', ['open']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [SettingsComponent],
      providers: [{ provide: MatSnackBar, useValue: snackBar }],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(SettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => expect(component).toBeTruthy());

  it('generalForm has default values', () => {
    expect(component.generalForm.value.apiBaseUrl).toBe('http://localhost:5000');
    expect(component.generalForm.value.pollIntervalSeconds).toBe(30);
    expect(component.generalForm.value.maxMetricHistory).toBe(500);
  });

  it('notificationForm has default values', () => {
    expect(component.notificationForm.value.enableEmailAlerts).toBeFalse();
    expect(component.notificationForm.value.enableCriticalOnly).toBeTrue();
  });

  it('saveGeneral opens snackbar', () => {
    component.saveGeneral();
    expect(snackBar.open).toHaveBeenCalledWith('General settings saved', 'Dismiss', { duration: 2000 });
  });

  it('saveNotifications opens snackbar', () => {
    component.saveNotifications();
    expect(snackBar.open).toHaveBeenCalledWith('Notification settings saved', 'Dismiss', { duration: 2000 });
  });
});
