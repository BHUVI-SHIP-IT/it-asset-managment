import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../../shared/components/base-table/base-table.component';

export interface Category {
  id: string;
  name: string;
}

export interface CreateCategoryCommand {
  name: string;
}

export interface UpdateCategoryCommand {
  id: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/categories';

  getCategories(pageNumber: number, pageSize: number, sortColumn?: string, sortDirection?: string): Observable<PaginatedResult<Category>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (sortColumn) {
      params = params.set('sortColumn', sortColumn);
    }
    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get<PaginatedResult<Category>>(this.baseUrl, { params });
  }

  getCategory(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.baseUrl}/${id}`);
  }

  createCategory(command: CreateCategoryCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, command);
  }

  updateCategory(id: string, command: UpdateCategoryCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  deleteCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
