import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { LoginComponent } from './login.component';
import { AuthService } from '../../../core/services/auth.service';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    authService = jasmine.createSpyObj('AuthService', ['login']);
    router = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [LoginComponent],
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => expect(component).toBeTruthy());

  it('form is invalid when empty', () => {
    expect(component.form.invalid).toBeTrue();
  });

  it('form is valid with email and password', () => {
    component.form.setValue({ email: 'a@b.com', password: 'pass' });
    expect(component.form.valid).toBeTrue();
  });

  it('submit does nothing when form is invalid', () => {
    component.submit();
    expect(authService.login).not.toHaveBeenCalled();
  });

  it('submit calls authService.login and navigates on success', () => {
    authService.login.and.returnValue(of({ token: 't', expiresAt: 'e' }));
    component.form.setValue({ email: 'a@b.com', password: 'pass' });
    component.submit();
    expect(authService.login).toHaveBeenCalledWith({ email: 'a@b.com', password: 'pass' });
    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('submit sets error on failure', () => {
    authService.login.and.returnValue(throwError(() => ({ error: { message: 'Bad credentials' } })));
    component.form.setValue({ email: 'a@b.com', password: 'pass' });
    component.submit();
    expect(component.error).toBe('Bad credentials');
    expect(component.loading).toBeFalse();
  });

  it('submit uses default error message when error.message absent', () => {
    authService.login.and.returnValue(throwError(() => ({})));
    component.form.setValue({ email: 'a@b.com', password: 'pass' });
    component.submit();
    expect(component.error).toBe('Login failed. Check your credentials.');
  });

  it('hidePassword is initially true', () => {
    expect(component.hidePassword).toBeTrue();
  });
});
