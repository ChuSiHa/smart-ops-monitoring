import { Pipe, PipeTransform } from '@angular/core';
import { HostStatus, ServiceNodeStatus, AlertSeverity, AlertStatus } from '../../core/models/models';

@Pipe({ name: 'hostStatus' })
export class HostStatusPipe implements PipeTransform {
  transform(value: HostStatus): string {
    switch (value) {
      case HostStatus.Online: return 'Online';
      case HostStatus.Offline: return 'Offline';
      case HostStatus.Maintenance: return 'Maintenance';
      default: return 'Unknown';
    }
  }
}

@Pipe({ name: 'serviceNodeStatus' })
export class ServiceNodeStatusPipe implements PipeTransform {
  transform(value: ServiceNodeStatus): string {
    switch (value) {
      case ServiceNodeStatus.Running: return 'Running';
      case ServiceNodeStatus.Stopped: return 'Stopped';
      case ServiceNodeStatus.Error: return 'Error';
      default: return 'Unknown';
    }
  }
}

@Pipe({ name: 'alertSeverity' })
export class AlertSeverityPipe implements PipeTransform {
  transform(value: AlertSeverity): string {
    switch (value) {
      case AlertSeverity.Critical: return 'Critical';
      case AlertSeverity.Warning: return 'Warning';
      default: return 'Info';
    }
  }
}

@Pipe({ name: 'alertStatus' })
export class AlertStatusPipe implements PipeTransform {
  transform(value: AlertStatus): string {
    switch (value) {
      case AlertStatus.Acknowledged: return 'Acknowledged';
      case AlertStatus.Resolved: return 'Resolved';
      default: return 'Open';
    }
  }
}
