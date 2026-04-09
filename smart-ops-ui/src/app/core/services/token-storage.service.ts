import { Injectable } from '@angular/core';

const TOKEN_KEY = 'som_token';
const EXPIRES_KEY = 'som_expires';

// ClaimTypes.Role serialised in JWT by .NET
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  saveToken(token: string, expiresAt: string): void {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(EXPIRES_KEY, expiresAt);
  }

  clearToken(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(EXPIRES_KEY);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    const exp = localStorage.getItem(EXPIRES_KEY);
    if (!exp) return true;
    return new Date(exp) > new Date();
  }

  /** Returns the list of roles embedded in the current JWT, or [] if none. */
  getRoles(): string[] {
    const token = this.getToken();
    if (!token) return [];
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const raw = payload[ROLE_CLAIM];
      if (!raw) return [];
      return Array.isArray(raw) ? raw : [raw];
    } catch {
      return [];
    }
  }

  /** Returns true when the current user has the given role. */
  hasRole(role: string): boolean {
    return this.getRoles().includes(role);
  }
}
