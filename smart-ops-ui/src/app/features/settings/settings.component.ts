import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss'],
})
export class SettingsComponent {
  generalForm: FormGroup;
  notificationForm: FormGroup;

  constructor(private fb: FormBuilder, private snackBar: MatSnackBar) {
    this.generalForm = this.fb.group({
      apiBaseUrl: ['http://localhost:5000'],
      pollIntervalSeconds: [30],
      maxMetricHistory: [500],
    });
    this.notificationForm = this.fb.group({
      enableEmailAlerts: [false],
      emailAddress: [''],
      enableCriticalOnly: [true],
    });
  }

  saveGeneral(): void {
    this.snackBar.open('General settings saved', 'Dismiss', { duration: 2000 });
  }

  saveNotifications(): void {
    this.snackBar.open('Notification settings saved', 'Dismiss', { duration: 2000 });
  }
}
