import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface NotificationDto {
  id: string;
  tenantId: string;
  userId?: string;
  type: string;
  message: string;
  data?: string;
  isRead: boolean;
  createdAtUtc: string;
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  totalPages: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);
  private apiUrl = '/api/v1/notifications';

  getNotifications(page = 1, pageSize = 50, unreadOnly = false): Observable<NotificationDto[]> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('unreadOnly', unreadOnly);
    
    return this.http.get<NotificationDto[]>(this.apiUrl, { params });
  }

  markRead(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/read`, {});
  }

  deleteNotification(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
