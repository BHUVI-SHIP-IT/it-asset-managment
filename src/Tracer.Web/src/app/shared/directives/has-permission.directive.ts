import { Directive, Input, TemplateRef, ViewContainerRef, effect, inject } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';

@Directive({
  selector: '[hasPermission]',
  standalone: true
})
export class HasPermissionDirective {
  private authService = inject(AuthService);
  private templateRef = inject(TemplateRef);
  private viewContainer = inject(ViewContainerRef);
  
  private permission = '';
  private hasView = false;

  @Input() set hasPermission(permission: string) {
    this.permission = permission;
    this.updateView();
  }

  constructor() {
    effect(() => {
      // Re-evaluate when permissions signal changes
      this.authService.permissions();
      this.updateView();
    });
  }

  private updateView() {
    const hasPerm = this.authService.hasPermission(this.permission);
    
    if (hasPerm && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!hasPerm && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }
}
