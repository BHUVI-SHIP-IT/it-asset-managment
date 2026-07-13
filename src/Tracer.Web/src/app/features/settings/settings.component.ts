import { Permissions } from '../../core/auth/permissions';
import { ToastService } from '../../core/ui/toast.service';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { SettingsService, SettingDto } from './settings.service';
import { HasPermissionDirective } from '../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    HasPermissionDirective
  ],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  readonly permissions = Permissions;

  private fb = inject(FormBuilder);
  private settingsService = inject(SettingsService);
  private toast = inject(ToastService);

  webhookForm!: FormGroup;
  smtpForm!: FormGroup;
  loading = signal(true);
  submitting = signal(false);

  ngOnInit(): void {
    this.initForms();
    this.loadSettings();
  }

  private initForms(): void {
    this.webhookForm = this.fb.group({
      SlackWebhookUrl: ['']
    });

    this.smtpForm = this.fb.group({
      SmtpHost: [''],
      SmtpPort: [''],
      SmtpUser: [''],
      SmtpPassword: ['']
    });
  }

  private loadSettings(): void {
    this.loading.set(true);
    this.settingsService.getAllSettings().subscribe({
      next: (settings: SettingDto[]) => {
        // Map settings to form controls
        const mappedSettings: Record<string, string | null> = {};
        settings.forEach(s => mappedSettings[s.key] = s.value);

        this.webhookForm.patchValue({
          SlackWebhookUrl: mappedSettings['SlackWebhookUrl'] || ''
        });

        this.smtpForm.patchValue({
          SmtpHost: mappedSettings['SmtpHost'] || '',
          SmtpPort: mappedSettings['SmtpPort'] || '',
          SmtpUser: mappedSettings['SmtpUser'] || '',
          SmtpPassword: mappedSettings['SmtpPassword'] || ''
        });

        this.loading.set(false);
      },
      error: () => {
        this.toast.showError('Error loading settings');
        this.loading.set(false);
      }
    });
  }

  saveWebhookSettings(): void {
    const value = this.webhookForm.value.SlackWebhookUrl;
    this.saveSetting('SlackWebhookUrl', value);
  }

  saveSmtpSettings(): void {
    this.submitting.set(true);
    
    // Quick loop to save all SMTP settings
    const keys = Object.keys(this.smtpForm.value);
    let completed = 0;
    
    keys.forEach(key => {
      this.settingsService.upsertSetting(key, this.smtpForm.value[key]).subscribe({
        next: () => {
          completed++;
          if (completed === keys.length) {
            this.toast.showSuccess('SMTP settings saved successfully');
            this.submitting.set(false);
          }
        },
        error: () => {
          this.toast.showError(`Error saving ${key}`);
          this.submitting.set(false);
        }
      });
    });
  }

  private saveSetting(key: string, value: string): void {
    this.submitting.set(true);
    this.settingsService.upsertSetting(key, value).subscribe({
      next: () => {
        this.toast.showSuccess('Setting saved successfully');
        this.submitting.set(false);
      },
      error: () => {
        this.toast.showError('Error saving setting');
        this.submitting.set(false);
      }
    });
  }
}
