import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../shared/components/base-table/base-table.component';

export interface UserDto {
  id: string;
  fullName: string;
  email: string;
  isActive: boolean;
  roleId: number;
  roleName: string;
  companyId: string;
}

export interface RoleDto {
  id: number;
  name: string;
}

export interface CreateUserCommand {
  fullName: string;
  email: string;
  password: string;
  roleId: number;
}

export interface AssignedItemDto {
  itemType: string;
  id: string;
  name: string;
  identifier: string | null;
  assignedAtUtc: string | null;
  status: string | null;
  detailPath: string | null;
}

export interface UserAssignedItemsDto {
  assets: AssignedItemDto[];
  consumables: AssignedItemDto[];
  components: AssignedItemDto[];
  accessories: AssignedItemDto[];
  licenses: AssignedItemDto[];
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/users';

  getUsers(pageNumber: number, pageSize: number): Observable<PaginatedResult<UserDto>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PaginatedResult<UserDto>>(this.baseUrl, { params });
  }

  getUser(id: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.baseUrl}/${id}`);
  }

  getAssignedItems(id: string): Observable<UserAssignedItemsDto> {
    return this.http.get<UserAssignedItemsDto>(`${this.baseUrl}/${id}/assigned-items`);
  }

  getMyAssignedItems(): Observable<UserAssignedItemsDto> {
    return this.http.get<UserAssignedItemsDto>('/api/v1/me/assigned-items');
  }

  getRoles(): Observable<RoleDto[]> {
    return this.http.get<RoleDto[]>(`${this.baseUrl}/roles`);
  }

  createUser(command: CreateUserCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, command);
  }
}
