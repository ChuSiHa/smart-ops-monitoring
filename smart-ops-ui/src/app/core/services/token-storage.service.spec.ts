import { TestBed } from '@angular/core/testing';
import { TokenStorageService } from './token-storage.service';

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

function makeJwt(payload: object): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
  const body = btoa(JSON.stringify(payload));
  return `${header}.${body}.signature`;
}

describe('TokenStorageService', () => {
  let service: TokenStorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TokenStorageService);
    localStorage.clear();
  });

  afterEach(() => localStorage.clear());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getToken', () => {
    it('returns null when no token stored', () => {
      expect(service.getToken()).toBeNull();
    });

    it('returns stored token', () => {
      localStorage.setItem('som_token', 'abc');
      expect(service.getToken()).toBe('abc');
    });
  });

  describe('saveToken', () => {
    it('saves token and expiry', () => {
      service.saveToken('mytoken', '2099-01-01T00:00:00Z');
      expect(localStorage.getItem('som_token')).toBe('mytoken');
      expect(localStorage.getItem('som_expires')).toBe('2099-01-01T00:00:00Z');
    });
  });

  describe('clearToken', () => {
    it('removes token and expiry', () => {
      service.saveToken('mytoken', '2099-01-01T00:00:00Z');
      service.clearToken();
      expect(localStorage.getItem('som_token')).toBeNull();
      expect(localStorage.getItem('som_expires')).toBeNull();
    });
  });

  describe('isLoggedIn', () => {
    it('returns false when no token', () => {
      expect(service.isLoggedIn()).toBeFalse();
    });

    it('returns true when token but no expiry', () => {
      localStorage.setItem('som_token', 'tok');
      expect(service.isLoggedIn()).toBeTrue();
    });

    it('returns true when token and future expiry', () => {
      service.saveToken('tok', '2099-12-31T00:00:00Z');
      expect(service.isLoggedIn()).toBeTrue();
    });

    it('returns false when token with past expiry', () => {
      service.saveToken('tok', '2000-01-01T00:00:00Z');
      expect(service.isLoggedIn()).toBeFalse();
    });
  });

  describe('getRoles', () => {
    it('returns empty array when no token', () => {
      expect(service.getRoles()).toEqual([]);
    });

    it('returns empty array when token has no role claim', () => {
      const token = makeJwt({ sub: 'user' });
      localStorage.setItem('som_token', token);
      expect(service.getRoles()).toEqual([]);
    });

    it('returns single role as array', () => {
      const token = makeJwt({ [ROLE_CLAIM]: 'Admin' });
      localStorage.setItem('som_token', token);
      expect(service.getRoles()).toEqual(['Admin']);
    });

    it('returns multiple roles', () => {
      const token = makeJwt({ [ROLE_CLAIM]: ['Admin', 'User'] });
      localStorage.setItem('som_token', token);
      expect(service.getRoles()).toEqual(['Admin', 'User']);
    });

    it('returns empty array on invalid token', () => {
      localStorage.setItem('som_token', 'not.a.valid.jwt');
      expect(service.getRoles()).toEqual([]);
    });
  });

  describe('hasRole', () => {
    it('returns false when no token', () => {
      expect(service.hasRole('Admin')).toBeFalse();
    });

    it('returns true when role matches', () => {
      const token = makeJwt({ [ROLE_CLAIM]: ['Admin', 'User'] });
      localStorage.setItem('som_token', token);
      expect(service.hasRole('Admin')).toBeTrue();
    });

    it('returns false when role does not match', () => {
      const token = makeJwt({ [ROLE_CLAIM]: ['User'] });
      localStorage.setItem('som_token', token);
      expect(service.hasRole('Admin')).toBeFalse();
    });
  });
});
