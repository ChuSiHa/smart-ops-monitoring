import { TestBed } from '@angular/core/testing';
import { Router, ActivatedRouteSnapshot } from '@angular/router';
import { RoleGuard } from './role.guard';
import { TokenStorageService } from '../services/token-storage.service';

describe('RoleGuard', () => {
  let guard: RoleGuard;
  let tokenStorage: jasmine.SpyObj<TokenStorageService>;
  let router: jasmine.SpyObj<Router>;

  function makeRoute(roles: string[]): ActivatedRouteSnapshot {
    const snapshot = new ActivatedRouteSnapshot();
    (snapshot as any).data = { roles };
    return snapshot;
  }

  beforeEach(() => {
    tokenStorage = jasmine.createSpyObj('TokenStorageService', ['hasRole']);
    router = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        RoleGuard,
        { provide: TokenStorageService, useValue: tokenStorage },
        { provide: Router, useValue: router },
      ],
    });

    guard = TestBed.inject(RoleGuard);
  });

  it('should be created', () => expect(guard).toBeTruthy());

  it('allows navigation when no roles required', () => {
    const route = makeRoute([]);
    expect(guard.canActivate(route)).toBeTrue();
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('allows navigation when user has required role', () => {
    tokenStorage.hasRole.and.callFake(r => r === 'Admin');
    const route = makeRoute(['Admin']);
    expect(guard.canActivate(route)).toBeTrue();
  });

  it('redirects to /dashboard when user lacks required role', () => {
    tokenStorage.hasRole.and.returnValue(false);
    const route = makeRoute(['Admin']);
    expect(guard.canActivate(route)).toBeFalse();
    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('allows when user has at least one of multiple required roles', () => {
    tokenStorage.hasRole.and.callFake(r => r === 'Operator');
    const route = makeRoute(['Admin', 'Operator']);
    expect(guard.canActivate(route)).toBeTrue();
  });
});
