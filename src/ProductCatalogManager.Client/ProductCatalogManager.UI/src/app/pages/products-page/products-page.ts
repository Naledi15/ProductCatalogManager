import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { BehaviorSubject, combineLatest } from 'rxjs';
import { debounceTime, map, startWith } from 'rxjs/operators';
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

  readonly loading$ = this.productService.products$.pipe(
    map(() => false),
    startWith(true),
    takeUntilDestroyed(),
  );

  readonly filteredProducts$ = combineLatest([
    this.productService.products$,
    this.searchTerm$.pipe(debounceTime(250), startWith('')),
    this.selectedCategoryId$,
  ]).pipe(
    map(([products, term, catId]) => {
      let result = products;
      const t = term.toLowerCase().trim();
      if (t) result = result.filter(p =>
        p.name.toLowerCase().includes(t) || p.description.toLowerCase().includes(t)
      );
      if (catId !== null) result = result.filter(p => p.categoryId === catId);
      return result;
    }),
    takeUntilDestroyed(),
  );

  onSearch(term: string): void { this.searchTerm$.next(term); }
  onCategoryChange(id: number | null): void { this.selectedCategoryId$.next(id); }

  onEdit(product: Product): void {
    this.router.navigate(['/products/edit', product.id]);
  }

  onDelete(id: number): void {
    this.productService.delete(id).subscribe();
  }
}
