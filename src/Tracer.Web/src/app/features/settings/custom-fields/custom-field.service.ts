import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CustomFieldDto {
  id: string;
  name: string;
  fieldType: 'text' | 'number' | 'date' | 'boolean' | 'dropdown';
  isRequired: boolean;
  options?: string; // Comma separated for dropdowns
}

@Injectable({
  providedIn: 'root'
})
export class CustomFieldService {
  private http = inject(HttpClient);
  private apiUrl = '/api/v1/custom-fields';

  getAllFields(): Observable<CustomFieldDto[]> {
    return this.http.get<CustomFieldDto[]>(this.apiUrl);
  }

  createField(field: Omit<CustomFieldDto, 'id'>): Observable<string> {
    return this.http.post<string>(this.apiUrl, field);
  }

  updateField(id: string, field: Omit<CustomFieldDto, 'id'>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, field);
  }

  deleteField(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
