import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ServiceNodeService } from './service-node.service';

describe('ServiceNodeService', () => {
  let service: ServiceNodeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    service = TestBed.inject(ServiceNodeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => expect(service).toBeTruthy());

  it('getByHost sends GET /api/servicenodes with hostId param', (done) => {
    service.getByHost('h1').subscribe(res => { expect(res).toEqual([]); done(); });
    const req = httpMock.expectOne(r => r.url === '/api/servicenodes');
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('hostId')).toBe('h1');
    req.flush([]);
  });

  it('create sends POST /api/servicenodes', (done) => {
    const cmd = { name: 'svc', type: 'web', hostId: 'h1' };
    service.create(cmd).subscribe(res => { expect(res).toBeTruthy(); done(); });
    const req = httpMock.expectOne('/api/servicenodes');
    expect(req.request.method).toBe('POST');
    req.flush({ id: '1', ...cmd });
  });
});
