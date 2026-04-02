import { Component, input, output } from '@angular/core';
import { Category } from '../../models/category.model';

@Component({
  selector: 'app-category-filter',
  templateUrl: './category-filter.html',
  styleUrl: './category-filter.scss',
})
export class CategoryFilterComponent {
  readonly categories = input.required<Category[]>();
  readonly selectedId = input<number | null>(null);
  readonly categoryChange = output<number | null>();

  onChange(value: string): void {
    this.categoryChange.emit(value ? +value : null);
  }
}
