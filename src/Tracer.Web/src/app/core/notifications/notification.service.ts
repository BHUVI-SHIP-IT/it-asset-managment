import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface NotificationDto {
  id: string;
  title: string;
  body: string;
  severity: string;
  channel: string;
  status: string;
  recipient?: string;
  failureReason?: string;
  isRead: boolean;
  sentAtUtc?: string;
  createdAtUtc: string;
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
}
