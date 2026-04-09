import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { TokenStorageService } from './token-storage.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let tokenStorage: jasmine.SpyObj<TokenStorageService>;

  beforeEach(() => {
    tokenStorage = jasmine.createSpyObj('TokenStorageService', [
      'isLoggedIn', 'saveToken', 'clearToken', 'getToken', 'getRoles', 'hasRole'
    ]);
    tokenStorage.isLoggedIn.and.returnValue(false);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: TokenStorageService, useValue: tokenStorage },
      ],
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('initializes isLoggedIn$ from tokenStorage', (done) => {
    tokenStorage.isLoggedIn.and.returnValue(true);
    const svc = TestBed.inject(AuthService);
    svc.isLoggedIn$.subscribe(v => {
      expect(v).toBeTrue();
      done();
    });
  });

  it('login sends POST and saves token', (done) => {
    const result = { token: 'tok', expiresAt: '2099-01-01' };
    service.login({ email: 'a@b.com', password: 'pass' }).subscribe(res => {
      expect(res).toEqual(result);
      expect(tokenStorage.saveToken).toHaveBeenCalledWith('tok', '2099-01-01');
      done();
    });
    const req = httpMock.expectOne('/api/auth/login');
    expect(req.request.method).toBe('POST');
    req.flush(result);
  });

  it('login updates isLoggedIn$ to true', (done) => {
    service.isLoggedIn$.subscribe(v => {
      if (v) { expect(v).toBeTrue(); done(); }
    });
    service.login({ email: 'a@b.com', password: 'pass' }).subscribe();
    httpMock.expectOne('/api/auth/login').flush({ token: 't', expiresAt: 'e' });
  });

  it('register sends POST', (done) => {
    service.register({ email: 'a@b.com', password: 'pass' }).subscribe(res => {
      expect(res).toEqual({ message: 'ok' });
      done();
    });
    const req = httpMock.expectOne('/api/auth/register');
    expect(req.request.method).toBe('POST');
    req.flush({ message: 'ok' });
  });

  it('logout clears token and updates isLoggedIn$', () => {
    service.logout();
    expect(tokenStorage.clearToken).toHaveBeenCalled();
  });

  it('isLoggedIn getter delegates to tokenStorage', () => {
    tokenStorage.isLoggedIn.and.returnValue(true);
    expect(service.isLoggedIn).toBeTrue();
  });
});
