import { Table, Badge, Card } from 'react-bootstrap';
import { Product } from '../types/product';

interface ProductListProps {
  products: Product[];
  isLoading: boolean;
}

const ProductList = ({ products, isLoading }: ProductListProps) => {
  if (isLoading) {
    return (
      <Card className="mb-4">
        <Card.Body className="text-center">
          <p>Loading products...</p>
        </Card.Body>
      </Card>
    );
  }

  if (products.length === 0) {
    return (
      <Card className="mb-4">
        <Card.Body className="text-center">
          <p>No products found. Try adjusting your search criteria.</p>
        </Card.Body>
      </Card>
    );
  }

  return (
    <Card className="mb-4">
      <Card.Header>
        <h4>Products ({products.length})</h4>
      </Card.Header>
      <Card.Body>
        <div className="table-responsive">
          <Table striped hover>
            <thead>
              <tr>
                <th>Name</th>
                <th>Category</th>
                <th>Price</th>
                <th>Status</th>
                <th>Tags</th>
              </tr>
            </thead>
            <tbody>
              {products.map((product) => (
                <tr key={product.id}>
                  <td>
                    <div className="fw-bold">{product.name}</div>
                    {product.description && (
                      <div className="small text-muted">{product.description.slice(0, 100)}
                        {product.description.length > 100 ? '...' : ''}
                      </div>
                    )}
                  </td>
                  <td>{product.category}</td>
                  <td>${product.price.toFixed(2)}</td>
                  <td>
                    <Badge bg={product.inStock ? 'success' : 'danger'}>
                      {product.inStock ? 'In Stock' : 'Out of Stock'}
                    </Badge>
                  </td>
                  <td>
                    <div className="d-flex flex-wrap gap-1">
                      {product.tags && product.tags.map((tag, index) => (
                        <Badge key={index} bg="secondary">{tag}</Badge>
                      ))}
                      {(!product.tags || product.tags.length === 0) && (
                        <span className="text-muted">-</span>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
        </div>
      </Card.Body>
    </Card>
  );
};

export default ProductList;