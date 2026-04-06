import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { BehaviorSubject, combineLatest } from 'rxjs';
import { debounceTime, distinctUntilChanged, startWith, switchMap } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { Product } from '../../models/product.model';
import { SearchBarComponent } from '../../components/search-bar/search-bar';
import { CategoryFilterComponent } from '../../components/category-filter/category-filter';
import { ProductListComponent } from '../../components/product-list/product-list';

interface SortOption { label: string; value: string; }

@Component({
  selector: 'app-products-page',
  imports: [SearchBarComponent, CategoryFilterComponent, ProductListComponent, RouterLink, AsyncPipe],
  templateUrl: './products-page.html',
  styleUrl: './products-page.scss',
})
export class ProductsPage {
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  private readonly searchTerm$ = new BehaviorSubject<string>('');
  readonly selectedCategoryId$ = new BehaviorSubject<number | null>(null);
  readonly sortBy$ = new BehaviorSubject<string | null>(null);

  readonly sortOptions: SortOption[] = [
    { label: 'Default',          value: '' },
    { label: 'Name A–Z',         value: 'name' },
    { label: 'Name Z–A',         value: 'name_desc' },
    { label: 'Price low–high',   value: 'price' },
    { label: 'Price high–low',   value: 'price_desc' },
    { label: 'SKU A–Z',          value: 'sku' },
    { label: 'SKU Z–A',          value: 'sku_desc' },
    { label: 'Qty low–high',     value: 'quantity' },
    { label: 'Qty high–low',     value: 'quantity_desc' },
    { label: 'Newest first',     value: 'created_desc' },
    { label: 'Oldest first',     value: 'created' },
  ];

  readonly categories$ = this.categoryService.categories$;
  readonly products$ = this.productService.products$;
  readonly loading$ = this.productService.loading$;

  constructor() {
    combineLatest([
      this.searchTerm$.pipe(debounceTime(300), distinctUntilChanged(), startWith('')),
      this.selectedCategoryId$,
      this.sortBy$,
    ]).pipe(
      switchMap(([search, categoryId, sortBy]) =>
        this.productService.loadProducts({ search: search || null, categoryId, sortBy, page: 1, pageSize: 20 }),
      ),
      takeUntilDestroyed(),
    ).subscribe();
  }

  onSearch(term: string): void { this.searchTerm$.next(term); }
  onCategoryChange(id: number | null): void { this.selectedCategoryId$.next(id); }
  onSortChange(value: string): void { this.sortBy$.next(value || null); }

  onEdit(product: Product): void {
    this.router.navigate(['/products/edit', product.id]);
  }

  onDelete(id: number): void {
    this.productService.delete(id).subscribe();
  }
}
