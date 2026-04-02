import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { EMPTY, switchMap, catchError } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-product-form',
  imports: [ReactiveFormsModule, AsyncPipe],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss',
})
export class ProductFormComponent {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly categories$ = this.categoryService.categories$;
  isEdit = signal(false);
  productId = signal<number | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    description: ['', [Validators.required, Validators.maxLength(500)]],
    sku: ['', [Validators.required, Validators.pattern(/^[A-Z0-9\-]+$/)]],
    price: [null as number | null, [Validators.required, Validators.min(0.01)]],
    quantity: [null as number | null, [Validators.required, Validators.min(0)]],
    categoryId: [null as number | null, Validators.required],
  });

  get f() { return this.form.controls; }

  constructor() {
    this.route.paramMap.pipe(
      switchMap(params => {
        const id = params.get('id');
        if (!id) {
          this.isEdit.set(false);
          this.productId.set(null);
          return EMPTY;
        }
        this.isEdit.set(true);
        this.productId.set(+id);
        this.loading.set(true);
        return this.productService.getById(+id).pipe(
          catchError(() => {
            this.error.set('Failed to load product.');
            this.loading.set(false);
            return EMPTY;
          }),
        );
      }),
      takeUntilDestroyed(),
    ).subscribe(product => {
      if (product) { this.form.patchValue(product); }
      this.loading.set(false);
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading.set(true);
    this.error.set(null);

    const v = this.form.value;
    const data = {
      name: v.name!,
      description: v.description!,
      sku: v.sku!,
      price: +v.price!,
      quantity: +v.quantity!,
      categoryId: +v.categoryId!,
    };

    const request$ = this.isEdit() && this.productId()
      ? this.productService.update(this.productId()!, data)
      : this.productService.create(data);

    request$.subscribe({
      next: () => { this.loading.set(false); this.router.navigate(['/products']); },
      error: () => { this.error.set('Failed to save product. Please try again.'); this.loading.set(false); },
    });
  }

  cancel(): void {
    this.router.navigate(['/products']);
  }
}
