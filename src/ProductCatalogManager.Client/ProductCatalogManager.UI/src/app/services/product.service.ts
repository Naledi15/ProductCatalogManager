import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map, of, delay } from 'rxjs';
import { Product, ProductFormData } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private _products$ = new BehaviorSubject<Product[]>([
    { id: 1, name: 'Laptop Pro', description: 'High-performance laptop for professionals', sku: 'LAP-001', price: 1299.99, quantity: 15, categoryId: 1, createdAt: new Date('2024-01-10'), updatedAt: new Date('2024-03-15') },
    { id: 2, name: 'Wireless Mouse', description: 'Ergonomic wireless mouse with long battery life', sku: 'MOU-002', price: 29.99, quantity: 100, categoryId: 2, createdAt: new Date('2024-01-12'), updatedAt: new Date('2024-02-20') },
    { id: 3, name: 'Mechanical Keyboard', description: 'RGB mechanical keyboard with tactile switches', sku: 'KBD-003', price: 89.99, quantity: 50, categoryId: 2, createdAt: new Date('2024-01-15'), updatedAt: new Date('2024-04-01') },
    { id: 4, name: 'Monitor 4K', description: '27-inch 4K IPS display with HDR support', sku: 'MON-004', price: 499.99, quantity: 25, categoryId: 1, createdAt: new Date('2024-02-01'), updatedAt: new Date('2024-03-10') },
    { id: 5, name: 'USB-C Hub', description: '7-in-1 USB-C hub for laptops and tablets', sku: 'HUB-005', price: 49.99, quantity: 75, categoryId: 3, createdAt: new Date('2024-02-08'), updatedAt: new Date('2024-02-08') },
  ]);

  readonly products$ = this._products$.asObservable();
  private nextId = 6;

  getById(id: number): Observable<Product | undefined> {
    return this.products$.pipe(map(products => products.find(p => p.id === id)));
  }

  create(data: ProductFormData): Observable<Product> {
    const now = new Date();
    const product: Product = { id: this.nextId++, ...data, createdAt: now, updatedAt: now };
    this._products$.next([...this._products$.value, product]);
    return of(product).pipe(delay(300));
  }

  update(id: number, data: ProductFormData): Observable<Product> {
    const existing = this._products$.value.find(p => p.id === id);
    const product: Product = { id, ...data, createdAt: existing?.createdAt ?? new Date(), updatedAt: new Date() };
    this._products$.next(this._products$.value.map(p => p.id === id ? product : p));
    return of(product).pipe(delay(300));
  }

  delete(id: number): Observable<void> {
    this._products$.next(this._products$.value.filter(p => p.id !== id));
    return of(void 0).pipe(delay(300));
  }
}
