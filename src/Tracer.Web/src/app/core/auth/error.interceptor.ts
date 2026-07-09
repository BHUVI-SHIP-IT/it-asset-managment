import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

/**
 * Centralizes HttpErrorResponse handling:
 *  - Normalizes the backend error shape ({code, description} or RFC problem-details
 *    {title, detail}) into err.error.message so component handlers show useful text.
 *  - On 401 (expired/invalid token mid-session) logs out and redirects to /login.
 *  - Surfaces 5xx / network failures via a snackbar so they are never silently swallowed.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const message = extractMessage(error);

      if (error.status === 0) {
        snackBar.open('Cannot reach the server. Check your connection.', 'Close', { duration: 5000 });
      } else if (error.status === 401) {
        // Do not bounce on the login request itself — let the form show the error.
        if (!req.url.includes('/auth/login')) {
          authService.logout();
          router.navigate(['/login']);
          snackBar.open('Your session has expired. Please sign in again.', 'Close', { duration: 5000 });
        }
      } else if (error.status === 403) {
        snackBar.open('You do not have permission to perform this action.', 'Close', { duration: 5000 });
      } else if (error.status >= 500) {
        snackBar.open(message || 'A server error occurred. Please try again.', 'Close', { duration: 5000 });
      }

      // Re-throw with a readable message so component-level error handlers can display it.
      return throwError(() => ({ ...error, message }));
    })
  );
};

function extractMessage(error: HttpErrorResponse): string {
  const body = error.error;
  if (body) {
    if (typeof body === 'string') return body;
    // Backend Result error: { code, description, type }
    if (body.description) return body.description;
    // RFC 7807 problem-details: { title, detail }
    if (body.detail) return body.detail;
    if (body.title) return body.title;
    // FluentValidation ModelState: { errors: { field: [msgs] } }
    if (body.errors && typeof body.errors === 'object') {
      const first = Object.values(body.errors)[0];
      if (Array.isArray(first) && first.length) return first[0] as string;
    }
  }
  return error.message || 'An unexpected error occurred.';
}
