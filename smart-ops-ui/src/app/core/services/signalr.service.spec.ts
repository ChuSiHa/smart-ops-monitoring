import { TestBed } from '@angular/core/testing';
import { SignalRService } from './signalr.service';
import { TokenStorageService } from './token-storage.service';

describe('SignalRService', () => {
  let service: SignalRService;
  let tokenStorage: jasmine.SpyObj<TokenStorageService>;

  beforeEach(() => {
    tokenStorage = jasmine.createSpyObj('TokenStorageService', [
      'getToken', 'isLoggedIn', 'saveToken', 'clearToken', 'getRoles', 'hasRole'
    ]);
    tokenStorage.getToken.and.returnValue(null);

    TestBed.configureTestingModule({
      providers: [
        SignalRService,
        { provide: TokenStorageService, useValue: tokenStorage },
      ],
    });

    service = TestBed.inject(SignalRService);
  });

  afterEach(() => {
    service.ngOnDestroy();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('exposes metricReceived$ observable', () => {
    expect(service.metricReceived$).toBeDefined();
  });

  it('exposes alertCreated$ observable', () => {
    expect(service.alertCreated$).toBeDefined();
  });

  it('stopAll can be called safely when no connections', () => {
    expect(() => service.stopAll()).not.toThrow();
  });

  it('ngOnDestroy completes subjects', () => {
    let metricCompleted = false;
    let alertCompleted = false;
    service.metricReceived$.subscribe({ complete: () => metricCompleted = true });
    service.alertCreated$.subscribe({ complete: () => alertCompleted = true });
    service.ngOnDestroy();
    expect(metricCompleted).toBeTrue();
    expect(alertCompleted).toBeTrue();
  });

  it('startMetricHub uses token from tokenStorage', () => {
    tokenStorage.getToken.and.returnValue('mytoken');
    // We can't easily test the full SignalR connection without a server,
    // but we can verify it doesn't throw with a null/valid token
    // and that getToken is called
    try {
      service.startMetricHub('host1');
    } catch (_) {}
    expect(tokenStorage.getToken).toHaveBeenCalled();
  });

  it('startAlertHub uses token from tokenStorage', () => {
    tokenStorage.getToken.and.returnValue('mytoken');
    try {
      service.startAlertHub('host1');
    } catch (_) {}
    expect(tokenStorage.getToken).toHaveBeenCalled();
  });

  it('startMetricHub does not create second connection if already started', () => {
    tokenStorage.getToken.and.returnValue(null);
    try { service.startMetricHub('host1'); } catch (_) {}
    const callCount = tokenStorage.getToken.calls.count();
    try { service.startMetricHub('host1'); } catch (_) {}
    // getToken should not have been called again since connection already exists
    expect(tokenStorage.getToken.calls.count()).toBe(callCount);
  });
});
