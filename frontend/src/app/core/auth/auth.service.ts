import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { User } from '../models/user.model';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';

const TOKEN_KEY = 'kudos_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly #token = signal<string | null>(null);
  readonly #user = signal<User | null>(null);

  readonly currentUser = this.#user.asReadonly();
  readonly isAuthenticated = computed(() => !!this.#token());
  readonly isAdmin = computed(() => this.#user()?.roles.includes('Admin') ?? false);

  constructor(private http: HttpClient, private router: Router) {
    this.loadFromStorage();
  }

  login(request: LoginRequest) {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, request)
      .pipe(tap(res => this.handleAuth(res)));
  }

  register(request: RegisterRequest) {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/register`, request)
      .pipe(tap(res => this.handleAuth(res)));
  }

  logout() {
    this.#token.set(null);
    this.#user.set(null);
    localStorage.removeItem(TOKEN_KEY);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this.#token();
  }

  private handleAuth(response: AuthResponse) {
    this.#token.set(response.token);
    this.#user.set(this.decodeToken(response.token));
    localStorage.setItem(TOKEN_KEY, response.token);
  }

  private loadFromStorage() {
    const token = localStorage.getItem(TOKEN_KEY);
    if (token && !this.isTokenExpired(token)) {
      this.#token.set(token);
      this.#user.set(this.decodeToken(token));
    } else {
      localStorage.removeItem(TOKEN_KEY);
    }
  }

  private decodeToken(token: string): User {
    const p = JSON.parse(atob(token.split('.')[1]));
    return {
      id: p.sub ?? p.nameid ?? p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ?? '',
      email: p.email ?? p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? '',
      fullName: p.name ?? p.unique_name ?? p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ?? '',
      department: p.department ?? '',
      roles: this.extractRoles(p),
    };
  }

  private extractRoles(payload: Record<string, unknown>): string[] {
    const roles = payload['role'] ?? payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? [];
    return Array.isArray(roles) ? roles : [roles as string];
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp * 1000 < Date.now();
    } catch {
      return true;
    }
  }
}
