import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastService } from '../ui/toast.service';
import { catchError, throwError } from 'rxjs';

/**
 * Centralizes HttpErrorResponse handling:
 *  - Normalizes the backend error shape ({code, description} or RFC problem-details
 *    {title, detail}) into err.error.message so component handlers show useful text.
 *  - Surfaces network / 403 / 5xx failures via snackbar.
 *  - Session expiry (401) is owned by authRefreshInterceptor — do not logout here,
 *    to avoid fighting refresh retries or double-redirecting on /auth/login|refresh.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const message = extractMessage(error);

      if (error.status === 0) {
        toast.showError('Cannot reach the server. Check your connection.');
      } else if (error.status === 403) {
        toast.showError('You do not have permission to perform this action.');
      } else if (error.status >= 500) {
        toast.showError(message || 'A server error occurred. Please try again.');
      }

      return throwError(() => ({ ...error, message }));
    })
  );
};

function extractMessage(error: HttpErrorResponse): string {
  const body = error.error;
  if (body) {
    if (typeof body === 'string') return body;
    if (body.description) return body.description;
    if (body.detail) return body.detail;
    if (body.title) return body.title;
    if (body.errors && typeof body.errors === 'object') {
      const first = Object.values(body.errors)[0];
      if (Array.isArray(first) && first.length) return first[0] as string;
    }
  }
  return error.message || 'An unexpected error occurred.';
}
