import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, catchError, of, map, finalize, shareReplay } from 'rxjs';
import { satisfiesAnyPermission, satisfiesPermission } from './permissions';
import { isEndUserRole } from './roles';

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);

  private tokenSignal = signal<string | null>(localStorage.getItem('token'));
  private refreshTokenSignal = signal<string | null>(localStorage.getItem('refreshToken'));
  private permissionsSignal = signal<string[]>(JSON.parse(localStorage.getItem('permissions') || '[]'));
  private roleSignal = signal<string | null>(localStorage.getItem('role'));

  /** Coalesces concurrent refresh attempts into a single in-flight request. */
  private refreshInFlight$: Observable<string | null> | null = null;

  public isAuthenticated = computed(() => !!this.tokenSignal());
  public permissions = computed(() => this.permissionsSignal());
  public role = computed(() => this.roleSignal());

  /** True when the JWT role is Employee/Guest (or missing) — personal dashboard only. */
  public isEndUser = computed(() => isEndUserRole(this.roleSignal()));

  /** True for SuperAdmin and other staff roles that use the org-wide admin dashboard. */
  public isAdminDashboardUser = computed(() => !this.isEndUser());

  constructor() {
    // Re-hydrate claims from the stored access token (covers pre-role localStorage sessions).
    const token = this.tokenSignal();
    if (token) {
      this.applyClaimsFromToken(token);
    }
  }

  login(email: string, password: string): Observable<TokenResponse | null> {
    return this.http.post<TokenResponse>('/api/v1/auth/login', { email, password }).pipe(
      tap(response => {
        if (response?.accessToken) {
          this.applyTokenResponse(response);
        }
      }),
      catchError(() => {
        this.logout();
        return of(null);
      })
    );
  }

  /**
   * Exchanges the stored refresh token for a new access (and rotated refresh) token.
   * Returns the new access token, or null if refresh is impossible / rejected.
   */
  refreshAccessToken(): Observable<string | null> {
    if (this.refreshInFlight$) {
      return this.refreshInFlight$;
    }

    const refreshToken = this.refreshTokenSignal() ?? localStorage.getItem('refreshToken');
    if (!refreshToken) {
      return of(null);
    }

    this.refreshInFlight$ = this.http
      .post<TokenResponse>('/api/v1/auth/refresh', { token: refreshToken })
      .pipe(
        tap(response => {
          if (response?.accessToken) {
            this.applyTokenResponse(response);
          }
        }),
        map(response => response?.accessToken ?? null),
        catchError(() => of(null)),
        finalize(() => {
          this.refreshInFlight$ = null;
        }),
        shareReplay(1)
      );

    return this.refreshInFlight$;
  }

  logout(): void {
    this.tokenSignal.set(null);
    this.refreshTokenSignal.set(null);
    this.permissionsSignal.set([]);
    this.roleSignal.set(null);
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('permissions');
    localStorage.removeItem('role');
  }

  getToken(): string | null {
    return this.tokenSignal();
  }

  getRefreshToken(): string | null {
    return this.refreshTokenSignal();
  }

  hasPermission(permission: string): boolean {
    return satisfiesPermission(this.permissionsSignal(), permission);
  }

  /** True if any of the given permissions is satisfied (OR). */
  hasAnyPermission(...permissions: string[]): boolean {
    return satisfiesAnyPermission(this.permissionsSignal(), permissions);
  }

  private applyTokenResponse(response: TokenResponse): void {
    this.tokenSignal.set(response.accessToken);
    localStorage.setItem('token', response.accessToken);

    if (response.refreshToken) {
      this.refreshTokenSignal.set(response.refreshToken);
      localStorage.setItem('refreshToken', response.refreshToken);
    }

    this.applyClaimsFromToken(response.accessToken);
  }

  private applyClaimsFromToken(accessToken: string): void {
    try {
      const payload = JSON.parse(atob(accessToken.split('.')[1]));
      const raw = payload.permissions ?? [];
      const perms = Array.isArray(raw) ? raw : [raw];
      this.permissionsSignal.set(perms);
      localStorage.setItem('permissions', JSON.stringify(perms));

      const role = typeof payload.role === 'string' ? payload.role : null;
      this.roleSignal.set(role);
      if (role) {
        localStorage.setItem('role', role);
      } else {
        localStorage.removeItem('role');
      }
    } catch (e) {
      console.error('Failed to parse JWT claims', e);
    }
  }
}
