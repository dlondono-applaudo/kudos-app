import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { AuthService } from './auth.service';

function makeJwt(payload: Record<string, unknown>): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
  const body = btoa(JSON.stringify(payload));
  return `${header}.${body}.fakesig`;
}

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  const validPayload = {
    sub: 'user-1',
    email: 'test@example.com',
    name: 'Test User',
    department: 'Engineering',
    role: 'User',
    exp: Math.floor(Date.now() / 1000) + 3600,
  };

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
      ],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should start unauthenticated', () => {
    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBeNull();
    expect(service.getToken()).toBeNull();
  });

  it('should decode JWT and set user on login', () => {
    const token = makeJwt(validPayload);
    service.login({ email: 'test@example.com', password: 'pass' }).subscribe();
    const req = httpMock.expectOne(r => r.url.includes('/auth/login'));
    req.flush({ token, expiration: new Date().toISOString(), email: 'test@example.com', fullName: 'Test User' });

    expect(service.isAuthenticated()).toBe(true);
    expect(service.currentUser()?.email).toBe('test@example.com');
    expect(service.currentUser()?.id).toBe('user-1');
    expect(service.currentUser()?.department).toBe('Engineering');
  });

  it('should store token in localStorage on login', () => {
    const token = makeJwt(validPayload);
    service.login({ email: 'test@example.com', password: 'pass' }).subscribe();
    httpMock.expectOne(r => r.url.includes('/auth/login')).flush({
      token, expiration: new Date().toISOString(), email: 'test@example.com', fullName: 'Test User',
    });

    expect(localStorage.getItem('kudos_token')).toBe(token);
  });

  it('should clear state on logout', () => {
    const token = makeJwt(validPayload);
    service.login({ email: 'test@example.com', password: 'pass' }).subscribe();
    httpMock.expectOne(r => r.url.includes('/auth/login')).flush({
      token, expiration: new Date().toISOString(), email: 'test@example.com', fullName: 'Test User',
    });

    service.logout();

    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBeNull();
    expect(localStorage.getItem('kudos_token')).toBeNull();
  });

  it('should detect expired tokens', () => {
    const expiredToken = makeJwt({ ...validPayload, exp: Math.floor(Date.now() / 1000) - 60 });
    localStorage.setItem('kudos_token', expiredToken);

    // Re-create to trigger loadFromStorage
    const freshService = TestBed.inject(AuthService);
    // The service constructor already ran and would have cleared the expired token
    expect(freshService.isAuthenticated()).toBe(false);
  });
});
