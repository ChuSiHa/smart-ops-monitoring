import { HostStatusPipe, ServiceNodeStatusPipe, AlertSeverityPipe, AlertStatusPipe } from './status.pipe';
import { HostStatus, ServiceNodeStatus, AlertSeverity, AlertStatus } from '../../core/models/models';

describe('HostStatusPipe', () => {
  const pipe = new HostStatusPipe();

  it('transforms Online', () => expect(pipe.transform(HostStatus.Online)).toBe('Online'));
  it('transforms Offline', () => expect(pipe.transform(HostStatus.Offline)).toBe('Offline'));
  it('transforms Maintenance', () => expect(pipe.transform(HostStatus.Maintenance)).toBe('Maintenance'));
  it('transforms Unknown default', () => expect(pipe.transform(HostStatus.Unknown)).toBe('Unknown'));
});

describe('ServiceNodeStatusPipe', () => {
  const pipe = new ServiceNodeStatusPipe();

  it('transforms Running', () => expect(pipe.transform(ServiceNodeStatus.Running)).toBe('Running'));
  it('transforms Stopped', () => expect(pipe.transform(ServiceNodeStatus.Stopped)).toBe('Stopped'));
  it('transforms Error', () => expect(pipe.transform(ServiceNodeStatus.Error)).toBe('Error'));
  it('transforms Unknown default', () => expect(pipe.transform(ServiceNodeStatus.Unknown)).toBe('Unknown'));
});

describe('AlertSeverityPipe', () => {
  const pipe = new AlertSeverityPipe();

  it('transforms Critical', () => expect(pipe.transform(AlertSeverity.Critical)).toBe('Critical'));
  it('transforms Warning', () => expect(pipe.transform(AlertSeverity.Warning)).toBe('Warning'));
  it('transforms Info (default)', () => expect(pipe.transform(AlertSeverity.Info)).toBe('Info'));
});

describe('AlertStatusPipe', () => {
  const pipe = new AlertStatusPipe();

  it('transforms Acknowledged', () => expect(pipe.transform(AlertStatus.Acknowledged)).toBe('Acknowledged'));
  it('transforms Resolved', () => expect(pipe.transform(AlertStatus.Resolved)).toBe('Resolved'));
  it('transforms Open (default)', () => expect(pipe.transform(AlertStatus.Open)).toBe('Open'));
});
