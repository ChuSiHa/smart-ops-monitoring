// ─── Enums ───────────────────────────────────────────────────────────────────

export enum HostStatus {
  Unknown = 0,
  Online = 1,
  Offline = 2,
  Maintenance = 3,
}

export enum ServiceNodeStatus {
  Unknown = 0,
  Running = 1,
  Stopped = 2,
  Error = 3,
}

export enum AlertSeverity {
  Info = 0,
  Warning = 1,
  Critical = 2,
}

export enum AlertStatus {
  Open = 0,
  Acknowledged = 1,
  Resolved = 2,
}

// ─── Response DTOs ────────────────────────────────────────────────────────────

export interface HostDto {
  id: string;
  name: string;
  ipAddress: string;
  osType: string;
  status: HostStatus;
  tags: string[];
  createdAt: string;
  updatedAt: string;
}

export interface ServiceNodeDto {
  id: string;
  name: string;
  type: string;
  hostId: string;
  status: ServiceNodeStatus;
  port: number | null;
  createdAt: string;
  updatedAt: string;
}

export interface MetricDto {
  id: string;
  hostId: string;
  serviceNodeId: string | null;
  metricType: string;
  value: number;
  unit: string;
  timestamp: string;
  labels: Record<string, string>;
}

export interface AlertDto {
  id: string;
  hostId: string;
  serviceNodeId: string | null;
  title: string;
  message: string;
  severity: AlertSeverity;
  status: AlertStatus;
  acknowledgedAt: string | null;
  resolvedAt: string | null;
  acknowledgedByUserId: string | null;
  createdAt: string;
}

export interface LoginResult {
  token: string;
  expiresAt: string;
}

export interface RegisterResult {
  message: string;
}

// ─── Request / Command types ──────────────────────────────────────────────────

export interface LoginCommand {
  email: string;
  password: string;
}

export interface RegisterCommand {
  email: string;
  password: string;
  displayName?: string;
}

export interface CreateHostCommand {
  name: string;
  ipAddress: string;
  osType: string;
  tags?: string[];
}

export interface CreateServiceNodeCommand {
  name: string;
  type: string;
  hostId: string;
  port?: number | null;
}

export interface IngestMetricCommand {
  hostId: string;
  serviceNodeId?: string | null;
  metricType: string;
  value: number;
  unit: string;
  labels?: Record<string, string>;
}

export interface CreateAlertCommand {
  hostId: string;
  serviceNodeId?: string | null;
  title: string;
  message: string;
  severity: AlertSeverity;
}

export interface UpdateAlertStatusCommand {
  status: AlertStatus;
}

export interface GetMetricsParams {
  metricType?: string;
  from?: string;
  to?: string;
}

export interface GetAlertsParams {
  hostId?: string;
  status?: string;
  severity?: string;
}
