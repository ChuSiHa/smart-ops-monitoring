import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AlertService } from './alert.service';
import { AlertSeverity, AlertStatus } from '../models/models';

describe('AlertService', () => {
  let service: AlertService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    service = TestBed.inject(AlertService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => expect(service).toBeTruthy());

  it('getAll without filter sends GET /api/alerts', (done) => {
    service.getAll().subscribe(res => { expect(res).toEqual([]); done(); });
    const req = httpMock.expectOne(r => r.url === '/api/alerts');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getAll with filters adds query params', (done) => {
    service.getAll({ hostId: 'h1', status: 'Open', severity: 'Critical' }).subscribe(() => done());
    const req = httpMock.expectOne(r => r.url === '/api/alerts');
    expect(req.request.params.get('hostId')).toBe('h1');
    expect(req.request.params.get('status')).toBe('Open');
    expect(req.request.params.get('severity')).toBe('Critical');
    req.flush([]);
  });

  it('create sends POST /api/alerts', (done) => {
    const cmd = { hostId: 'h1', title: 'T', message: 'M', severity: AlertSeverity.Warning };
    service.create(cmd).subscribe(res => { expect(res).toBeTruthy(); done(); });
    const req = httpMock.expectOne('/api/alerts');
    expect(req.request.method).toBe('POST');
    req.flush({ id: '1', ...cmd, status: AlertStatus.Open, createdAt: '' });
  });

  it('updateStatus sends PATCH /api/alerts/:id/status', (done) => {
    service.updateStatus('id1', { status: AlertStatus.Acknowledged }).subscribe(() => done());
    const req = httpMock.expectOne('/api/alerts/id1/status');
    expect(req.request.method).toBe('PATCH');
    req.flush({ id: 'id1' });
  });
});
