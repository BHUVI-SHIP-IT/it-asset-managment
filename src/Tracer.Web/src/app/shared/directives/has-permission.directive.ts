import { Directive, Input, TemplateRef, ViewContainerRef, effect, inject } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';
import { satisfiesAnyPermission } from '../../core/auth/permissions';

/**
 * Structural directive that shows the host template only when the current user
 * satisfies the required permission(s). Pass a string or string[] (OR).
 *
 * @example
 * ```html
 * <button *hasPermission="'Assets.Delete'">Delete</button>
 * <a *hasPermission="['Requests.ViewOwn', 'Requests.ViewAll']">Requests</a>
 * ```
 */
@Directive({
  selector: '[hasPermission]',
  standalone: true
})
export class HasPermissionDirective {
  private authService = inject(AuthService);
  private templateRef = inject(TemplateRef<unknown>);
  private viewContainer = inject(ViewContainerRef);

  private required: string | string[] = '';
  private hasView = false;

  @Input() set hasPermission(permission: string | string[]) {
    this.required = permission ?? '';
    this.updateView();
  }

  constructor() {
    effect(() => {
      // Re-evaluate when the permissions signal changes (login / refresh / logout).
      this.authService.permissions();
      this.updateView();
    });
  }

  private updateView(): void {
    const perms = this.authService.permissions();
    const allowed = satisfiesAnyPermission(perms, this.required);

    if (allowed && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!allowed && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }
}
