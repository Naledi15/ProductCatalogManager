import { Routes } from '@angular/router';
import { ProductsPage } from './pages/products-page/products-page';
import { ProductFormComponent } from './components/product-form/product-form';
import { CategoriesPage } from './pages/categories-page/categories-page';

export const routes: Routes = [
  { path: '', redirectTo: '/products', pathMatch: 'full' },
  { path: 'products', component: ProductsPage },
  { path: 'products/new', component: ProductFormComponent },
  { path: 'products/edit/:id', component: ProductFormComponent },
  { path: 'categories', component: CategoriesPage },
];
