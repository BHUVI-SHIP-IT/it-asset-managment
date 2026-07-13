import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject, NgZone } from '@angular/core';
import { Router } from '@angular/router';
import { ToastService } from '../ui/toast.service';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';

const AUTH_PATHS = ['/auth/login', '/auth/refresh'];
const RETRY_HEADER = 'X-Auth-Retry';

/**
 * On 401 for authenticated API calls:
 *  1. Attempt a single token refresh (coalesced in AuthService).
 *  2. Retry the original request once with the new access token.
 *  3. If refresh fails, clear session and redirect to login.
 *
 * Skips login/refresh endpoints and already-retried requests to avoid loops.
 */
export const authRefreshInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const toast = inject(ToastService);
  const ngZone = inject(NgZone);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401) {
        return throwError(() => error);
      }

      const isAuthEndpoint = AUTH_PATHS.some(path => req.url.includes(path));
      const alreadyRetried = req.headers.has(RETRY_HEADER);

      // Never attempt refresh for login/refresh themselves — avoids infinite loops.
      if (isAuthEndpoint) {
        return throwError(() => error);
      }

      // Retry already used a fresh token and still got 401 — session is unrecoverable.
      if (alreadyRetried) {
        forceReLogin(authService, router, toast, ngZone);
        return throwError(() => error);
      }

      return authService.refreshAccessToken().pipe(
        switchMap(newToken => {
          if (!newToken) {
            forceReLogin(authService, router, toast, ngZone);
            return throwError(() => error);
          }

          const retryReq = req.clone({
            setHeaders: {
              Authorization: `Bearer ${newToken}`,
              [RETRY_HEADER]: '1'
            }
          });
          return next(retryReq);
        })
      );
    })
  );
};

function forceReLogin(
  authService: AuthService,
  router: Router,
  toast: ToastService,
  ngZone: NgZone
): void {
  authService.logout();
  // HTTP/RxJS callbacks can miss a reliable CD tick with event coalescing —
  // force navigation inside the Angular zone so the login outlet paints immediately.
  ngZone.run(() => {
    void router.navigateByUrl('/login', { replaceUrl: true });
  });
  toast.showInfo('Your session has expired, please log in again');
}
