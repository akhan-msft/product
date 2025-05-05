import axios from 'axios';
import { Product, AddProduct, SearchProduct } from '../types/product';
import { API_BASE_URL } from '../constants/apiConfig';

// Create axios instance with default config
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Product service for handling all API calls
export const ProductService = {
  // Get all products
  getAllProducts: async (): Promise<Product[]> => {
    try {
      const response = await apiClient.get<Product[]>('');
      return response.data;
    } catch (error) {
      console.error('Error fetching products:', error);
      throw error;
    }
  },

  // Get a single product by ID
  getProductById: async (id: string): Promise<Product> => {
    try {
      const response = await apiClient.get<Product>(`/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching product with ID ${id}:`, error);
      throw error;
    }
  },

  // Create a new product
  createProduct: async (product: AddProduct): Promise<Product> => {
    try {
      const response = await apiClient.post<Product>('', product);
      return response.data;
    } catch (error) {
      console.error('Error creating product:', error);
      throw error;
    }
  },

  // Update an existing product
  updateProduct: async (id: string, product: AddProduct): Promise<Product> => {
    try {
      const response = await apiClient.put<Product>(`/${id}`, product);
      return response.data;
    } catch (error) {
      console.error(`Error updating product with ID ${id}:`, error);
      throw error;
    }
  },

  // Delete a product
  deleteProduct: async (id: string): Promise<boolean> => {
    try {
      await apiClient.delete(`/${id}`);
      return true;
    } catch (error) {
      console.error(`Error deleting product with ID ${id}:`, error);
      throw error;
    }
  },

  // Search for products based on criteria
  searchProducts: async (searchCriteria: SearchProduct): Promise<Product[]> => {
    try {
      const response = await apiClient.post<Product[]>('/search', searchCriteria);
      return response.data;
    } catch (error) {
      console.error('Error searching products:', error);
      throw error;
    }
  },
};