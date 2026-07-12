import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface SettingDto {
  key: string;
  value: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private http = inject(HttpClient);
  private apiUrl = '/api/v1/settings';

  getAllSettings(): Observable<SettingDto[]> {
    return this.http.get<SettingDto[]>(this.apiUrl);
  }

  upsertSetting(key: string, value: string | null): Observable<string> {
    return this.http.put<string>(`${this.apiUrl}/${encodeURIComponent(key)}`, { value });
  }
}
