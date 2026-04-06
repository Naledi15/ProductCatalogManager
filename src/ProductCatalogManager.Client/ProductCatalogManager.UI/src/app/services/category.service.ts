import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, tap } from 'rxjs';
import { Category, CategoryFormData } from '../models/category.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/categories`;

  private _categories$ = new BehaviorSubject<Category[]>([]);

  readonly categories$ = this._categories$.asObservable();

  constructor() {
    this.http.get<Category[]>(this.base).subscribe(cats => this._categories$.next(cats));
  }

  create(data: CategoryFormData): Observable<Category> {
    return this.http.post<Category>(this.base, data).pipe(
      tap(cat => this._categories$.next([...this._categories$.value, cat])),
    );
  }

  update(id: number, data: CategoryFormData): Observable<Category> {
    const category: Category = { id, ...data };
    this._categories$.next(this._categories$.value.map(c => c.id === id ? category : c));
    return of(category);
  }

  delete(id: number): Observable<void> {
    this._categories$.next(this._categories$.value.filter(c => c.id !== id));
    return of(void 0);
  }
}
