import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../../shared/components/base-table/base-table.component';

export interface AssetModel {
  id: string;
  name: string;
}

export interface CreateAssetModelCommand {
  name: string;
}

export interface UpdateAssetModelCommand {
  id: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class AssetModelService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/assetmodels';

  getAssetModels(pageNumber: number, pageSize: number, sortColumn?: string, sortDirection?: string): Observable<PaginatedResult<AssetModel>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (sortColumn) {
      params = params.set('sortColumn', sortColumn);
    }
    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get<PaginatedResult<AssetModel>>(this.baseUrl, { params });
  }

  getAssetModel(id: string): Observable<AssetModel> {
    return this.http.get<AssetModel>(`${this.baseUrl}/${id}`);
  }

  createAssetModel(command: CreateAssetModelCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, command);
  }

  updateAssetModel(id: string, command: UpdateAssetModelCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  deleteAssetModel(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
