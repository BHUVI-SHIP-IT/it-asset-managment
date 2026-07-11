import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../shared/components/base-table/base-table.component';

export interface AssetDto {
  id: string;
  assetTag: string;
  name: string;
  serialNumber?: string;
  status: string;
  assetModelId: string;
  statusLabelId: number;
  locationId?: string;
  assignedUserId?: string;
  purchaseCost: number;
  checkedOutAtUtc?: string;
}

export interface AssetDetailDto {
  id: string;
  assetTag: string;
  name: string;
  serialNumber?: string;
  status: string;
  notes?: string;
  companyId: string;
  assetModelId: string;
  statusLabelId: number;
  locationId?: string;
  assignedUserId?: string;
  purchaseCost: number;
  purchaseDate?: string;
  checkedOutAtUtc?: string;
  lastCheckinAtUtc?: string;
  createdAtUtc: string;
  createdBy?: string;
  updatedAtUtc?: string;
  updatedBy?: string;
}

export interface CreateAssetCommand {
  assetTag: string;
  name: string;
  assetModelId: string;
  statusLabelId: number;
  purchaseCost: number;
  locationId?: string;
  serialNumber?: string;
  purchaseDate?: string;
  depreciationId?: string;
}

export interface UpdateAssetCommand {
  id: string;
  name: string;
  assetModelId: string;
  statusLabelId: number;
  purchaseCost: number;
  locationId?: string;
  serialNumber?: string;
  purchaseDate?: string;
  depreciationId?: string;
  notes?: string;
}

export interface CheckoutAssetCommand {
  assetId: string;
  userId: string;
}

export interface CheckinAssetCommand {
  assetId: string;
}

@Injectable({
  providedIn: 'root'
})
export class AssetService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/assets';

  getAssets(
    pageNumber: number, 
    pageSize: number, 
    sortColumn?: string, 
    sortDirection?: string,
    searchTerm?: string,
    status?: string,
    statusLabelId?: number,
    locationId?: string
  ): Observable<PaginatedResult<AssetDto>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (sortColumn) params = params.set('sortBy', sortColumn);
    if (sortDirection) params = params.set('sortDescending', (sortDirection === 'desc').toString());
    if (searchTerm) params = params.set('searchTerm', searchTerm);
    if (status) params = params.set('status', status);
    if (statusLabelId) params = params.set('statusLabelId', statusLabelId.toString());
    if (locationId) params = params.set('locationId', locationId);

    return this.http.get<PaginatedResult<AssetDto>>(this.baseUrl, { params });
  }

  getAsset(id: string): Observable<AssetDetailDto> {
    return this.http.get<AssetDetailDto>(`${this.baseUrl}/${id}`);
  }

  createAsset(command: CreateAssetCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, command);
  }

  updateAsset(id: string, command: UpdateAssetCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  deleteAsset(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  checkoutAsset(id: string, command: CheckoutAssetCommand): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/checkout`, command);
  }

  checkinAsset(id: string, command: CheckinAssetCommand): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/checkin`, command);
  }

  getAssetHistory(id: string): Observable<AssetHistoryDto[]> {
    return this.http.get<AssetHistoryDto[]>(`${this.baseUrl}/${id}/history`);
  }
}

export interface AssetHistoryDto {
  id: string;
  assetTag: string;
  name: string;
  status: string;
  statusLabelId: number;
  assignedUserId?: string;
  validFrom: string;
  validTo: string;
}
