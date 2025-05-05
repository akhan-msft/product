export interface Product {
  id: string;
  name: string;
  description?: string;
  price: number;
  category: string;
  tags?: string[];
  inStock: boolean;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface AddProduct {
  name: string;
  description?: string;
  price: number;
  category: string;
  tags?: string[];
  inStock: boolean;
}

export interface SearchProduct {
  query?: string;
  category?: string;
}