import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, catchError, of } from 'rxjs';

export interface LoginResponse {
  accessToken: string;
  expiresIn: number;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  
  // State
  private tokenSignal = signal<string | null>(localStorage.getItem('token'));
  private permissionsSignal = signal<string[]>(JSON.parse(localStorage.getItem('permissions') || '[]'));

  // Computed
  public isAuthenticated = computed(() => !!this.tokenSignal());
  public permissions = computed(() => this.permissionsSignal());

  login(email: string, password: string): Observable<LoginResponse | null> {
    return this.http.post<LoginResponse>('/api/v1/auth/login', { email, password }).pipe(
      tap(response => {
        if (response && response.accessToken) {
          this.tokenSignal.set(response.accessToken);
          localStorage.setItem('token', response.accessToken);
          
          try {
            // Decode JWT payload to extract permissions
            const payload = JSON.parse(atob(response.accessToken.split('.')[1]));
            const perms = payload.permissions || [];
            this.permissionsSignal.set(perms);
            localStorage.setItem('permissions', JSON.stringify(perms));
          } catch (e) {
            console.error('Failed to parse JWT permissions', e);
          }
        }
      }),
      catchError(() => {
        this.logout();
        return of(null);
      })
    );
  }

  logout(): void {
    this.tokenSignal.set(null);
    this.permissionsSignal.set([]);
    localStorage.removeItem('token');
    localStorage.removeItem('permissions');
  }

  getToken(): string | null {
    return this.tokenSignal();
  }

  hasPermission(permission: string): boolean {
    return this.permissionsSignal().includes(permission);
  }
}
