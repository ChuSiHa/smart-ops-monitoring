import { Injectable } from '@angular/core';

const TOKEN_KEY = 'som_token';
const EXPIRES_KEY = 'som_expires';

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
}
