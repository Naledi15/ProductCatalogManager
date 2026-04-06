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

  readonly categories$ = this.categoryService.categories$;
  readonly products$ = this.productService.products$;
  readonly loading$ = this.productService.loading$;

  constructor() {
    combineLatest([
      this.searchTerm$.pipe(debounceTime(300), distinctUntilChanged(), startWith('')),
      this.selectedCategoryId$,
    ]).pipe(
      switchMap(([search, categoryId]) =>
        this.productService.loadProducts({ search: search || null, categoryId, page: 1, pageSize: 20 }),
      ),
      takeUntilDestroyed(),
    ).subscribe();
  }

  onSearch(term: string): void { this.searchTerm$.next(term); }
  onCategoryChange(id: number | null): void { this.selectedCategoryId$.next(id); }

  onEdit(product: Product): void {
    this.router.navigate(['/products/edit', product.id]);
  }

  onDelete(id: number): void {
    this.productService.delete(id).subscribe();
  }
}
