import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { BehaviorSubject, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CategoryService } from '../../services/category.service';
import { Category } from '../../models/category.model';
import { ConfirmDialogComponent } from '../../components/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-categories-page',
  imports: [ReactiveFormsModule, ConfirmDialogComponent, AsyncPipe],
  templateUrl: './categories-page.html',
  styleUrl: './categories-page.scss',
})
export class CategoriesPage {
  private categoryService = inject(CategoryService);
  private fb = inject(FormBuilder);

  loading = signal(false);
  showForm = signal(false);
  private editingId$ = new BehaviorSubject<number | null>(null);
  confirmDeleteId = signal<number | null>(null);
  error = signal<string | null>(null);

  readonly categories$ = this.categoryService.categories$;

  readonly parentOptions$ = combineLatest([
    this.categoryService.categories$,
    this.editingId$,
  ]).pipe(
    map(([cats, editId]) => cats.filter(c => c.id !== editId)),
    takeUntilDestroyed(),
  );

  form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    description: ['', Validators.required],
    parentCategoryId: [null as number | null],
  });

  get f() { return this.form.controls; }
  get editingId(): number | null { return this.editingId$.value; }

  getParentName(parentId: number | null, categories: Category[]): string | null {
    if (parentId === null) return null;
    return categories.find(c => c.id === parentId)?.name ?? null;
  }

  startAdd(): void {
    this.editingId$.next(null);
    this.form.reset({ parentCategoryId: null });
    this.showForm.set(true);
  }

  startEdit(category: Category): void {
    this.editingId$.next(category.id);
    this.form.patchValue(category);
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
    this.editingId$.next(null);
    this.form.reset();
    this.error.set(null);
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading.set(true);

    const v = this.form.value;
    const data = {
      name: v.name!,
      description: v.description!,
      parentCategoryId: v.parentCategoryId ? +v.parentCategoryId : null,
    };

    const request$ = this.editingId
      ? this.categoryService.update(this.editingId, data)
      : this.categoryService.create(data);

    request$.subscribe({
      next: () => { this.loading.set(false); this.cancelForm(); },
      error: () => { this.error.set('Failed to save category.'); this.loading.set(false); },
    });
  }

  onDelete(id: number): void {
    this.confirmDeleteId.set(id);
  }

  confirmDelete(): void {
    const id = this.confirmDeleteId();
    if (id !== null) {
      this.categoryService.delete(id).subscribe();
      this.confirmDeleteId.set(null);
    }
  }
}
