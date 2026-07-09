import { Directive, inject, signal, computed, effect } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Directive()
export abstract class BaseTableComponent<T> {
  protected http = inject(HttpClient);
  
  // State
  public data = signal<T[]>([]);
  public totalCount = signal<number>(0);
  public loading = signal<boolean>(false);
  
  public pageIndex = signal<number>(0);
  public pageSize = signal<number>(10);
  public sortActive = signal<string>('');
  public sortDirection = signal<'asc' | 'desc' | ''>('');

  constructor() {
    // Automatically fetch data when pagination or sorting changes
    effect(() => {
      this.loadData();
    });
  }

  // To be implemented by child classes
  protected abstract fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<T>>;

  public loadData(): void {
    this.loading.set(true);
    
    // Convert 0-indexed paginator to 1-indexed API
    const pageNumber = this.pageIndex() + 1;
    const size = this.pageSize();
    const sort = this.sortActive();
    const order = this.sortDirection();

    this.fetchData(pageNumber, size, sort, order).subscribe({
      next: (result) => {
        this.data.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.data.set([]);
        this.totalCount.set(0);
        this.loading.set(false);
      }
    });
  }

  public onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
  }

  public onSortChange(sort: Sort): void {
    this.sortActive.set(sort.active);
    this.sortDirection.set(sort.direction);
    // Reset to first page when sorting changes
    this.pageIndex.set(0);
  }
}
