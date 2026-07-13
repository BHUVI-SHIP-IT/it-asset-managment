import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { satisfiesAnyPermission } from './permissions';

/**
 * Route guard factory: allows activation when the user satisfies any of the
 * given permissions (OR). On failure, redirects to /not-authorized.
 *
 * Usage: `canActivate: [permissionGuard(Permissions.Assets.View)]`
 *        `canActivate: [permissionGuard(Permissions.A.View, Permissions.B.View)]`
 */
export function permissionGuard(...required: string[]): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (required.length === 0) {
      return true;
    }

    if (satisfiesAnyPermission(auth.permissions(), required)) {
      return true;
    }

    return router.createUrlTree(['/not-authorized']);
  };
}
