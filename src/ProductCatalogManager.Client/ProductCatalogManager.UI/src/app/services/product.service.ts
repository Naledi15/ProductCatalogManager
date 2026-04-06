import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, tap, catchError, throwError } from 'rxjs';
import { Product, ProductFormData, PagedResponse, ProductQueryParams } from '../models/product.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/products`;

  private _products$ = new BehaviorSubject<Product[]>([]);
  private _loading$ = new BehaviorSubject<boolean>(false);
  private _totalCount$ = new BehaviorSubject<number>(0);
  private _totalPages$ = new BehaviorSubject<number>(0);
  private _currentParams: ProductQueryParams = {};

  readonly products$ = this._products$.asObservable();
  readonly loading$ = this._loading$.asObservable();
  readonly totalCount$ = this._totalCount$.asObservable();
  readonly totalPages$ = this._totalPages$.asObservable();

  loadProducts(params: ProductQueryParams = {}): Observable<PagedResponse<Product>> {
    this._currentParams = params;
    this._loading$.next(true);

    let httpParams = new HttpParams();
    if (params.page != null) httpParams = httpParams.set('page', params.page);
    if (params.pageSize != null) httpParams = httpParams.set('pageSize', params.pageSize);
    if (params.categoryId != null) httpParams = httpParams.set('categoryId', params.categoryId);
    if (params.search) httpParams = httpParams.set('search', params.search);
    if (params.minPrice != null) httpParams = httpParams.set('minPrice', params.minPrice);
    if (params.maxPrice != null) httpParams = httpParams.set('maxPrice', params.maxPrice);
    if (params.inStock) httpParams = httpParams.set('inStock', true);
    if (params.sortBy) httpParams = httpParams.set('sortBy', params.sortBy);

    return this.http.get<PagedResponse<Product>>(this.base, { params: httpParams }).pipe(
      tap(response => {
        this._products$.next(response.items);
        this._totalCount$.next(response.totalCount);
        this._totalPages$.next(response.totalPages);
        this._loading$.next(false);
      }),
      catchError(err => {
        this._loading$.next(false);
        return throwError(() => err);
      }),
    );
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.base}/${id}`);
  }

  create(data: ProductFormData): Observable<Product> {
    return this.http.post<Product>(this.base, data).pipe(
      tap(() => this.loadProducts(this._currentParams).subscribe()),
    );
  }

  update(id: number, data: ProductFormData): Observable<Product> {
    return this.http.put<Product>(`${this.base}/${id}`, data).pipe(
      tap(() => this.loadProducts(this._currentParams).subscribe()),
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`).pipe(
      tap(() => this.loadProducts(this._currentParams).subscribe()),
    );
  }
}
