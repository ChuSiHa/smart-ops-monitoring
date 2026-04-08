import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { LoginCommand, LoginResult, RegisterCommand, RegisterResult } from '../models/models';
import { TokenStorageService } from './token-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _isLoggedIn$ = new BehaviorSubject<boolean>(false);
  readonly isLoggedIn$ = this._isLoggedIn$.asObservable();

  constructor(
    private http: HttpClient,
    private tokenStorage: TokenStorageService
  ) {
    this._isLoggedIn$.next(tokenStorage.isLoggedIn());
  }

  login(command: LoginCommand): Observable<LoginResult> {
    return this.http.post<LoginResult>('/api/auth/login', command).pipe(
      tap((res) => {
        this.tokenStorage.saveToken(res.token, res.expiresAt);
        this._isLoggedIn$.next(true);
      })
    );
  }

  register(command: RegisterCommand): Observable<RegisterResult> {
    return this.http.post<RegisterResult>('/api/auth/register', command);
  }

  logout(): void {
    this.tokenStorage.clearToken();
    this._isLoggedIn$.next(false);
  }

  get isLoggedIn(): boolean {
    return this.tokenStorage.isLoggedIn();
  }
}
