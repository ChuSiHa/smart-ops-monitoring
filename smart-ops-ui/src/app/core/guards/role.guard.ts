import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { TokenStorageService } from '../services/token-storage.service';

/**
 * Route guard that enforces role-based access control.
 *
 * Apply by setting `data: { roles: ['Admin'] }` on a route alongside `AuthGuard`.
 * Users who lack any of the required roles are redirected to `/dashboard`.
 *
 * If no `roles` array is provided in route data the guard allows access (behaves
 * like a no-op so it is safe to apply globally).
 */
@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
  constructor(private tokenStorage: TokenStorageService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const requiredRoles: string[] = route.data['roles'] ?? [];
    if (requiredRoles.length === 0) return true;

    const hasRole = requiredRoles.some((r) => this.tokenStorage.hasRole(r));
    if (!hasRole) {
      this.router.navigate(['/dashboard']);
      return false;
    }
    return true;
  }
}
