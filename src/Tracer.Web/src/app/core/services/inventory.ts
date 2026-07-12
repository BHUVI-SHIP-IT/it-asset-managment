import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ConsumableDto {
  id: number;
  name: string;
  companyId: string;
  totalQuantity: number;
  purchaseCost: number;
}

export interface LicenseDto {
  id: string;
  name: string;
  companyId: string;
  manufacturerId?: string;
  totalSeats: number;
  purchaseCost: number;
  expirationDate?: string;
  notes?: string;
}

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private http = inject(HttpClient);
  private apiUrl = '/api/v1';

  // Consumables
  getConsumables(): Observable<ConsumableDto[]> {
    return this.http.get<ConsumableDto[]>(`${this.apiUrl}/consumables`);
  }

  createConsumable(data: { name: string; totalQuantity: number; purchaseCost: number }): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/consumables`, data);
  }

  checkoutConsumable(
    id: number,
    checkoutData: { consumableId: number; assignedToUserId: string; quantity: number }
  ): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/consumables/${id}/checkout`, checkoutData);
  }

  // Licenses
  getLicenses(): Observable<LicenseDto[]> {
    return this.http.get<LicenseDto[]>(`${this.apiUrl}/licenses`);
  }

  getLicense(id: string): Observable<LicenseDto> {
    return this.http.get<LicenseDto>(`${this.apiUrl}/licenses/${id}`);
  }

  createLicense(data: Partial<LicenseDto>): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/licenses`, data);
  }
}
