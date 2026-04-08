import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject, merge } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

import { AlertService } from '../../core/services/alert.service';
import { HostService } from '../../core/services/host.service';
import { SignalRService } from '../../core/services/signalr.service';
import {
  AlertDto,
  AlertSeverity,
  AlertStatus,
  HostDto,
  GetAlertsParams,
} from '../../core/models/models';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.scss'],
})
export class AlertsComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  displayedColumns = ['title', 'severity', 'status', 'host', 'createdAt', 'actions'];
  alerts: AlertDto[] = [];
  hosts: HostDto[] = [];
  loading = true;
  showCreateForm = false;

  // Filters
  filterStatus = '';
  filterSeverity = '';
  filterHostId = '';

  AlertSeverity = AlertSeverity;
  AlertStatus = AlertStatus;

  createForm: FormGroup;

  constructor(
    private alertService: AlertService,
    private hostService: HostService,
    private signalRService: SignalRService,
    private snackBar: MatSnackBar,
    private fb: FormBuilder
  ) {
    this.createForm = this.fb.group({
      hostId: ['', Validators.required],
      title: ['', Validators.required],
      message: ['', Validators.required],
      severity: [AlertSeverity.Info, Validators.required],
    });
  }

  ngOnInit(): void {
    this.hostService.getAll().pipe(takeUntil(this.destroy$)).subscribe((h) => {
      this.hosts = h;
    });

    this.loadAlerts();

    merge(this.signalRService.alertCreated$)
      .pipe(takeUntil(this.destroy$))
      .subscribe((alert) => {
        this.alerts = [alert, ...this.alerts];
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadAlerts(): void {
    this.loading = true;
    const params: GetAlertsParams = {};
    if (this.filterStatus) params.status = this.filterStatus;
    if (this.filterSeverity) params.severity = this.filterSeverity;
    if (this.filterHostId) params.hostId = this.filterHostId;

    this.alertService
      .getAll(params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (alerts) => {
          this.alerts = alerts;
          this.loading = false;
        },
        error: () => { this.loading = false; },
      });
  }

  acknowledge(alert: AlertDto): void {
    this.alertService
      .updateStatus(alert.id, { status: AlertStatus.Acknowledged })
      .subscribe({
        next: (updated) => {
          this.alerts = this.alerts.map((a) => (a.id === updated.id ? updated : a));
          this.snackBar.open('Alert acknowledged', 'Dismiss', { duration: 3000 });
        },
      });
  }

  resolve(alert: AlertDto): void {
    this.alertService
      .updateStatus(alert.id, { status: AlertStatus.Resolved })
      .subscribe({
        next: (updated) => {
          this.alerts = this.alerts.map((a) => (a.id === updated.id ? updated : a));
          this.snackBar.open('Alert resolved', 'Dismiss', { duration: 3000 });
        },
      });
  }

  createAlert(): void {
    if (this.createForm.invalid) return;
    const v = this.createForm.value;
    this.alertService
      .create({
        hostId: v.hostId!,
        title: v.title!,
        message: v.message!,
        severity: v.severity!,
      })
      .subscribe({
        next: (a) => {
          this.alerts = [a, ...this.alerts];
          this.createForm.reset({ severity: AlertSeverity.Info });
          this.showCreateForm = false;
          this.snackBar.open('Alert created', 'Dismiss', { duration: 3000 });
        },
      });
  }

  hostName(hostId: string): string {
    return this.hosts.find((h) => h.id === hostId)?.name ?? hostId.slice(0, 8);
  }
}

