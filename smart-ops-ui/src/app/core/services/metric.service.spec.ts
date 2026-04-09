import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MetricService } from './metric.service';

describe('MetricService', () => {
  let service: MetricService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    service = TestBed.inject(MetricService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => expect(service).toBeTruthy());

  it('ingest sends POST /api/metrics/ingest', (done) => {
    const cmd = { hostId: 'h1', metricType: 'cpu', value: 42, unit: '%' };
    service.ingest(cmd).subscribe(res => { expect(res).toBeTruthy(); done(); });
    const req = httpMock.expectOne('/api/metrics/ingest');
    expect(req.request.method).toBe('POST');
    req.flush({ id: '1', ...cmd });
  });

  it('getByHost without filter sends GET /api/metrics/host/:id', (done) => {
    service.getByHost('h1').subscribe(res => { expect(res).toEqual([]); done(); });
    const req = httpMock.expectOne(r => r.url === '/api/metrics/host/h1');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getByHost with filter adds query params', (done) => {
    service.getByHost('h1', { metricType: 'cpu', from: '2024-01-01', to: '2024-01-02' }).subscribe(() => done());
    const req = httpMock.expectOne(r => r.url === '/api/metrics/host/h1');
    expect(req.request.params.get('metricType')).toBe('cpu');
    expect(req.request.params.get('from')).toBe('2024-01-01');
    expect(req.request.params.get('to')).toBe('2024-01-02');
    req.flush([]);
  });
});
