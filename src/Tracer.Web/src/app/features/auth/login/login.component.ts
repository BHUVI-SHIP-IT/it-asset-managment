import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/auth/auth.service';
import { ToastService } from '../../../core/ui/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  template: `
    <div class="login-container">
      <div class="login-left">
        <div class="decorative-circle circle-1"></div>
        <div class="decorative-circle circle-2"></div>
        <h1 class="left-title">Tracer IAM</h1>
        <p class="left-subtitle">
          Enterprise IT Asset Management made simple. Track hardware, manage software licenses, 
          and automate your financials securely from a single pane of glass.
        </p>
      </div>
      <div class="login-right">
        <div class="login-form-container">
          <mat-icon class="brand-icon">inventory_2</mat-icon>
          <h2 class="welcome-text">Welcome back</h2>
          <p class="subtitle-text">Please enter your credentials to access your account.</p>

          <form (ngSubmit)="onLogin()" #loginForm="ngForm">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email address</mat-label>
              <input matInput type="email" name="email" [(ngModel)]="email" required placeholder="admin@tracer.io" id="login-email">
              <mat-icon matSuffix>email</mat-icon>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Password</mat-label>
              <input matInput [type]="hidePassword() ? 'password' : 'text'" name="password" [(ngModel)]="password" required id="login-password">
              <button mat-icon-button matSuffix type="button" (click)="hidePassword.set(!hidePassword())">
                <mat-icon>{{ hidePassword() ? 'visibility_off' : 'visibility' }}</mat-icon>
              </button>
            </mat-form-field>

            <button mat-flat-button type="submit" class="login-btn" [disabled]="loading() || !loginForm.valid">
              @if (loading()) {
                <mat-spinner diameter="20" color="accent"></mat-spinner>
                <span>Authenticating...</span>
              } @else {
                Sign In
              }
            </button>
          </form>

          <p class="footer-note">© 2026 Tracer IAM · All rights reserved.</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      min-height: 100vh;
      background-color: #ffffff;
    }
    
    .login-container {
      display: flex;
      min-height: 100vh;
      width: 100%;
    }
    
    .login-left {
      display: none;
    }
    
    @media (min-width: 900px) {
      .login-left {
        display: flex;
        flex: 1.2;
        background: linear-gradient(135deg, #0f172a 0%, #1e3a8a 100%);
        color: white;
        flex-direction: column;
        justify-content: center;
        padding: 10%;
        position: relative;
        overflow: hidden;
      }
    }
    
    .login-right {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      background-color: #ffffff;
      padding: 24px;
    }
    
    .login-form-container {
      width: 100%;
      max-width: 400px;
    }
    
    .brand-icon {
      font-size: 48px;
      height: 48px;
      width: 48px;
      margin-bottom: 24px;
      color: #2563eb;
    }
    
    .welcome-text {
      font-size: 32px;
      font-weight: 700;
      color: #111827;
      margin: 0 0 8px 0;
      letter-spacing: -0.5px;
    }
    
    .subtitle-text {
      font-size: 15px;
      color: #6b7280;
      margin: 0 0 32px 0;
    }
    
    .full-width {
      width: 100%;
      margin-bottom: 12px;
    }
    
    .login-btn {
      width: 100%;
      height: 48px;
      font-size: 16px;
      font-weight: 500;
      margin-top: 16px;
      border-radius: 8px !important;
      background-color: #2563eb !important;
      color: #ffffff !important;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      box-shadow: 0 4px 6px -1px rgba(37, 99, 235, 0.2), 0 2px 4px -1px rgba(37, 99, 235, 0.1) !important;
      transition: all 0.2s ease-in-out;
    }
    
    .login-btn:hover:not([disabled]) {
      background-color: #1d4ed8 !important;
      box-shadow: 0 10px 15px -3px rgba(37, 99, 235, 0.3), 0 4px 6px -2px rgba(37, 99, 235, 0.1) !important;
      transform: translateY(-1px);
    }
    
    .login-btn[disabled] {
      background-color: #93c5fd !important;
      box-shadow: none !important;
      color: rgba(255, 255, 255, 0.8) !important;
    }
    
    .footer-note {
      text-align: center;
      font-size: 13px;
      color: #9ca3af;
      margin-top: 48px;
    }
    
    .left-title {
      font-size: 56px;
      font-weight: 800;
      line-height: 1.1;
      margin: 0 0 24px 0;
      z-index: 1;
      letter-spacing: -1px;
    }
    
    .left-subtitle {
      font-size: 18px;
      color: #bfdbfe;
      line-height: 1.6;
      max-width: 480px;
      z-index: 1;
      margin: 0;
      font-weight: 400;
    }
    
    .decorative-circle {
      position: absolute;
      border-radius: 50%;
      background: rgba(255, 255, 255, 0.03);
      pointer-events: none;
    }
    
    .circle-1 {
      width: 800px;
      height: 800px;
      top: -200px;
      left: -200px;
      border: 1px solid rgba(255, 255, 255, 0.05);
    }
    
    .circle-2 {
      width: 600px;
      height: 600px;
      bottom: -150px;
      right: -150px;
      background: linear-gradient(135deg, rgba(255,255,255,0.05) 0%, transparent 100%);
    }
    
    /* Ensure Angular Material inputs fit the modern style */
    ::ng-deep .mat-mdc-text-field-wrapper {
      background-color: #f9fafb !important;
    }
    ::ng-deep .mdc-notched-outline__leading,
    ::ng-deep .mdc-notched-outline__notch,
    ::ng-deep .mdc-notched-outline__trailing {
      border-color: #e5e7eb !important;
    }
    ::ng-deep .mat-mdc-form-field.mat-focused .mdc-notched-outline__leading,
    ::ng-deep .mat-mdc-form-field.mat-focused .mdc-notched-outline__notch,
    ::ng-deep .mat-mdc-form-field.mat-focused .mdc-notched-outline__trailing {
      border-color: #2563eb !important;
      border-width: 2px !important;
    }
  `]
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private toast = inject(ToastService);

  email = 'admin@tracer.io';
  password = 'Admin123!';
  loading = signal(false);
  hidePassword = signal(true);

  onLogin(): void {
    this.loading.set(true);
    this.authService.login(this.email, this.password).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res) {
          this.router.navigate(['/dashboard']);
        } else {
          this.toast.showError('Invalid credentials. Please try again.');
        }
      },
      error: () => {
        this.loading.set(false);
        this.toast.showError('Login failed. Check your credentials.');
      }
    });
  }
}
