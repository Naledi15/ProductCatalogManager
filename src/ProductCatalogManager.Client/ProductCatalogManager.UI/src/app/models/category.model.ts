export interface Category {
  id: number;
  name: string;
  description: string;
  parentCategoryId: number | null;
}

export type CategoryFormData = Omit<Category, 'id'>;
