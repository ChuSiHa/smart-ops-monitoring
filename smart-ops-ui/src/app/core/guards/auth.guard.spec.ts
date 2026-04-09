import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthGuard } from './auth.guard';
import { TokenStorageService } from '../services/token-storage.service';

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let tokenStorage: jasmine.SpyObj<TokenStorageService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    tokenStorage = jasmine.createSpyObj('TokenStorageService', ['isLoggedIn']);
    router = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        AuthGuard,
        { provide: TokenStorageService, useValue: tokenStorage },
        { provide: Router, useValue: router },
      ],
    });

    guard = TestBed.inject(AuthGuard);
  });

  it('should be created', () => expect(guard).toBeTruthy());

  it('allows navigation when logged in', () => {
    tokenStorage.isLoggedIn.and.returnValue(true);
    expect(guard.canActivate()).toBeTrue();
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('redirects to /auth/login when not logged in', () => {
    tokenStorage.isLoggedIn.and.returnValue(false);
    expect(guard.canActivate()).toBeFalse();
    expect(router.navigate).toHaveBeenCalledWith(['/auth/login']);
  });
});
