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

  getRoles(): Observable<RoleDto[]> {
    return this.http.get<RoleDto[]>(`${this.baseUrl}/roles`);
  }

  createUser(command: CreateUserCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, command);
  }
}
