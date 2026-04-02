import { Component, input, output, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { Product } from '../../models/product.model';
import { Category } from '../../models/category.model';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-product-list',
  imports: [CurrencyPipe, ConfirmDialogComponent],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss',
})
export class ProductListComponent {
  readonly products = input.required<Product[]>();
  readonly categories = input.required<Category[]>();
  readonly loading = input(false);
  readonly edit = output<Product>();
  readonly delete = output<number>();

  viewMode = signal<'grid' | 'table'>('grid');
  confirmDeleteId = signal<number | null>(null);

  getCategoryName(categoryId: number): string {
    return this.categories().find(c => c.id === categoryId)?.name ?? 'Uncategorized';
  }

  onDelete(id: number): void {
    this.confirmDeleteId.set(id);
  }

  confirmDelete(): void {
    const id = this.confirmDeleteId();
    if (id !== null) {
      this.delete.emit(id);
      this.confirmDeleteId.set(null);
    }
  }
}
