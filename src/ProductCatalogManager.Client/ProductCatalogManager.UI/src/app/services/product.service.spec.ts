import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';
import { ProductService } from './product.service';

describe('ProductService', () => {
  let service: ProductService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProductService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return initial products', async () => {
    const products = await firstValueFrom(service.products$);
    expect(products.length).toBeGreaterThan(0);
  });

  it('should create a product with createdAt and updatedAt timestamps', async () => {
    const initial = await firstValueFrom(service.products$);
    const initialCount = initial.length;

    await firstValueFrom(service.create({
      name: 'Test Product',
      description: 'A test description',
      sku: 'TST-001',
      price: 9.99,
      quantity: 10,
      categoryId: 1,
    }));

    const updated = await firstValueFrom(service.products$);
    expect(updated.length).toBe(initialCount + 1);
    const created = updated[updated.length - 1];
    expect(created.name).toBe('Test Product');
    expect(created.sku).toBe('TST-001');
    expect(created.createdAt).toBeInstanceOf(Date);
    expect(created.updatedAt).toBeInstanceOf(Date);
  });

  it('should update a product and refresh updatedAt', async () => {
    const before = await firstValueFrom(service.products$);
    const original = before.find(p => p.id === 1)!;

    await firstValueFrom(service.update(1, {
      name: 'Updated Laptop',
      description: 'Updated description',
      sku: 'LAP-001',
      price: 999.99,
      quantity: 10,
      categoryId: 1,
    }));

    const products = await firstValueFrom(service.products$);
    const updated = products.find(p => p.id === 1)!;
    expect(updated.name).toBe('Updated Laptop');
    expect(updated.createdAt).toEqual(original.createdAt);
    expect(updated.updatedAt.getTime()).toBeGreaterThanOrEqual(original.updatedAt.getTime());
  });

  it('should delete a product', async () => {
    const initial = await firstValueFrom(service.products$);
    const initialCount = initial.length;

    await firstValueFrom(service.delete(1));

    const updated = await firstValueFrom(service.products$);
    expect(updated.length).toBe(initialCount - 1);
    expect(updated.find(p => p.id === 1)).toBeUndefined();
  });

  it('should get product by id', async () => {
    const product = await firstValueFrom(service.getById(1));
    expect(product).toBeDefined();
    expect(product?.id).toBe(1);
    expect(product?.sku).toBeDefined();
  });
});
