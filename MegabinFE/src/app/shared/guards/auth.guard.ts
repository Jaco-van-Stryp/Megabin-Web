import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthTokenService } from '../../services/auth-token.service';
import { UserRoles } from '../../services/model/userRoles';

/**
 * Guard that checks if the user is authenticated
 * Redirects to login if not authenticated
 */
export const authGuard: CanActivateFn = () => {
  const authTokenService = inject(AuthTokenService);
  const router = inject(Router);

  if (authTokenService.isAuthenticated()) {
    return true;
  }

  return router.createUrlTree(['/login']);
};

/**
 * Factory function that creates a guard for checking user roles
 * @param requiredRole The role required to access the route
 */
export const roleGuard = (requiredRole: UserRoles): CanActivateFn => {
  return () => {
    const authTokenService = inject(AuthTokenService);
    const router = inject(Router);
    const user = authTokenService.user();

    if (user?.role === requiredRole) {
      return true;
    }

    // Redirect to appropriate dashboard based on actual role, or login if no role
    if (user?.role) {
      switch (user.role) {
        case UserRoles.Admin:
          return router.createUrlTree(['/admin/dashboard']);
        case UserRoles.Driver:
          return router.createUrlTree(['/driver']);
        case UserRoles.Customer:
          return router.createUrlTree(['/customer']);
        default:
          return router.createUrlTree(['/login']);
      }
    }

    return router.createUrlTree(['/login']);
  };
};
