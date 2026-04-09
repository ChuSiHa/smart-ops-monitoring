import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HostService } from './host.service';

describe('HostService', () => {
  let service: HostService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    service = TestBed.inject(HostService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => expect(service).toBeTruthy());

  it('getAll sends GET /api/hosts', (done) => {
    service.getAll().subscribe(res => { expect(res).toEqual([]); done(); });
    const req = httpMock.expectOne('/api/hosts');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getById sends GET /api/hosts/:id', (done) => {
    service.getById('abc').subscribe(res => { expect(res).toBeTruthy(); done(); });
    const req = httpMock.expectOne('/api/hosts/abc');
    expect(req.request.method).toBe('GET');
    req.flush({ id: 'abc', name: 'Host1' });
  });

  it('create sends POST /api/hosts', (done) => {
    const cmd = { name: 'H', ipAddress: '1.1.1.1', osType: 'Linux', tags: [] };
    service.create(cmd).subscribe(res => { expect(res).toBeTruthy(); done(); });
    const req = httpMock.expectOne('/api/hosts');
    expect(req.request.method).toBe('POST');
    req.flush({ id: '1', ...cmd });
  });
});
