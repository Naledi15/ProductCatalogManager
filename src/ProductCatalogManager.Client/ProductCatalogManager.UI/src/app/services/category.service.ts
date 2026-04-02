import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, delay } from 'rxjs';
import { Category, CategoryFormData } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private _categories$ = new BehaviorSubject<Category[]>([
    { id: 1, name: 'Electronics', description: 'Electronic devices and accessories', parentCategoryId: null },
    { id: 2, name: 'Peripherals', description: 'Computer peripherals and accessories', parentCategoryId: 1 },
    { id: 3, name: 'Accessories', description: 'General accessories and add-ons', parentCategoryId: null },
  ]);

  readonly categories$ = this._categories$.asObservable();
  private nextId = 4;

  create(data: CategoryFormData): Observable<Category> {
    const category: Category = { id: this.nextId++, ...data };
    this._categories$.next([...this._categories$.value, category]);
    return of(category).pipe(delay(300));
  }

  update(id: number, data: CategoryFormData): Observable<Category> {
    const category: Category = { id, ...data };
    this._categories$.next(this._categories$.value.map(c => c.id === id ? category : c));
    return of(category).pipe(delay(300));
  }

  delete(id: number): Observable<void> {
    this._categories$.next(this._categories$.value.filter(c => c.id !== id));
    return of(void 0).pipe(delay(300));
  }
}
