import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { RegisterComponent } from './register.component';
import { AuthService } from '../../../core/services/auth.service';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    authService = jasmine.createSpyObj('AuthService', ['register']);
    router = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [RegisterComponent],
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => expect(component).toBeTruthy());

  it('form is invalid when email/password missing', () => {
    expect(component.form.invalid).toBeTrue();
  });

  it('form is valid with email and password', () => {
    component.form.setValue({ displayName: '', email: 'a@b.com', password: 'password123' });
    expect(component.form.valid).toBeTrue();
  });

  it('submit does nothing when form invalid', () => {
    component.submit();
    expect(authService.register).not.toHaveBeenCalled();
  });

  it('submit calls authService.register on valid form', () => {
    authService.register.and.returnValue(of({ message: 'ok' }));
    component.form.setValue({ displayName: 'John', email: 'a@b.com', password: 'password123' });
    component.submit();
    expect(authService.register).toHaveBeenCalled();
  });

  it('sets success message and navigates after 1500ms', fakeAsync(() => {
    authService.register.and.returnValue(of({ message: 'Registration successful!' }));
    component.form.setValue({ displayName: '', email: 'a@b.com', password: 'password123' });
    component.submit();
    expect(component.success).toBe('Registration successful!');
    tick(1500);
    expect(router.navigate).toHaveBeenCalledWith(['/auth/login']);
  }));

  it('sets error on registration failure', () => {
    authService.register.and.returnValue(throwError(() => ({ error: { message: 'Email taken' } })));
    component.form.setValue({ displayName: '', email: 'a@b.com', password: 'password123' });
    component.submit();
    expect(component.error).toBe('Email taken');
    expect(component.loading).toBeFalse();
  });

  it('uses default error message when error.message absent', () => {
    authService.register.and.returnValue(throwError(() => ({})));
    component.form.setValue({ displayName: '', email: 'a@b.com', password: 'password123' });
    component.submit();
    expect(component.error).toBe('Registration failed.');
  });

  it('uses default success message when res.message is empty', () => {
    authService.register.and.returnValue(of({ message: '' }));
    component.form.setValue({ displayName: '', email: 'a@b.com', password: 'password123' });
    component.submit();
    expect(component.success).toBe('Registration successful!');
  });
});
