import { useState, FormEvent } from 'react';
import { Form, Button, Row, Col, Card } from 'react-bootstrap';
import { AddProduct } from '../types/product';

interface ProductFormProps {
  onSubmit: (product: AddProduct) => void;
  isLoading?: boolean;
}

const ProductForm = ({ onSubmit, isLoading = false }: ProductFormProps) => {
  const [product, setProduct] = useState<AddProduct>({
    name: '',
    description: '',
    price: 0,
    category: '',
    tags: [],
    inStock: true
  });

  const [tag, setTag] = useState<string>('');
  const [validated, setValidated] = useState(false);

  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const form = e.currentTarget;
    
    if (form.checkValidity() === false) {
      e.stopPropagation();
      setValidated(true);
      return;
    }

    onSubmit(product);
    // Don't reset form here - let the parent component decide when to reset
    // after a successful submission
  };

  const handleInputChange = (name: keyof AddProduct, value: string | number | boolean) => {
    setProduct(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const addTag = () => {
    if (tag.trim() && !product.tags?.includes(tag.trim())) {
      setProduct(prev => ({
        ...prev,
        tags: [...(prev.tags || []), tag.trim()]
      }));
      setTag('');
    }
  };

  const removeTag = (tagToRemove: string) => {
    setProduct(prev => ({
      ...prev,
      tags: prev.tags?.filter(t => t !== tagToRemove)
    }));
  };

  return (
    <Card>
      <Card.Header>
        <h4>Create New Product</h4>
      </Card.Header>
      <Card.Body>
        <Form noValidate validated={validated} onSubmit={handleSubmit}>
          <Row className="mb-3">
            <Col md={6}>
              <Form.Group>
                <Form.Label>Name *</Form.Label>
                <Form.Control
                  required
                  type="text"
                  placeholder="Product name"
                  value={product.name}
                  onChange={(e) => handleInputChange('name', e.target.value)}
                />
                <Form.Control.Feedback type="invalid">
                  Product name is required
                </Form.Control.Feedback>
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group>
                <Form.Label>Category *</Form.Label>
                <Form.Control
                  required
                  type="text"
                  placeholder="Product category"
                  value={product.category}
                  onChange={(e) => handleInputChange('category', e.target.value)}
                />
                <Form.Control.Feedback type="invalid">
                  Category is required
                </Form.Control.Feedback>
              </Form.Group>
            </Col>
          </Row>

          <Row className="mb-3">
            <Col md={12}>
              <Form.Group>
                <Form.Label>Description</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={3}
                  placeholder="Product description"
                  value={product.description || ''}
                  onChange={(e) => handleInputChange('description', e.target.value)}
                />
              </Form.Group>
            </Col>
          </Row>

          <Row className="mb-3">
            <Col md={6}>
              <Form.Group>
                <Form.Label>Price *</Form.Label>
                <Form.Control
                  required
                  type="number"
                  min="0"
                  step="0.01"
                  placeholder="Product price"
                  value={product.price}
                  onChange={(e) => handleInputChange('price', parseFloat(e.target.value))}
                />
                <Form.Control.Feedback type="invalid">
                  Please provide a valid price
                </Form.Control.Feedback>
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group className="mt-4">
                <Form.Check
                  type="checkbox"
                  label="In Stock"
                  checked={product.inStock}
                  onChange={(e) => handleInputChange('inStock', e.target.checked)}
                />
              </Form.Group>
            </Col>
          </Row>

          <Row className="mb-3">
            <Col md={12}>
              <Form.Group>
                <Form.Label>Tags</Form.Label>
                <div className="d-flex">
                  <Form.Control
                    type="text"
                    placeholder="Add tag"
                    value={tag}
                    onChange={(e) => setTag(e.target.value)}
                  />
                  <Button 
                    variant="outline-secondary" 
                    className="ms-2" 
                    onClick={addTag}
                    type="button"
                  >
                    Add
                  </Button>
                </div>
              </Form.Group>
            </Col>
          </Row>

          {product.tags && product.tags.length > 0 && (
            <Row className="mb-3">
              <Col>
                <div className="d-flex flex-wrap gap-2">
                  {product.tags.map((tagItem, index) => (
                    <span 
                      key={index} 
                      className="badge bg-secondary d-flex align-items-center"
                    >
                      {tagItem}
                      <Button 
                        variant="link" 
                        className="p-0 ps-1 text-light" 
                        onClick={() => removeTag(tagItem)}
                      >
                        &times;
                      </Button>
                    </span>
                  ))}
                </div>
              </Col>
            </Row>
          )}

          <Button 
            variant="primary" 
            type="submit"
            disabled={isLoading}
          >
            {isLoading ? 'Saving...' : 'Create Product'}
          </Button>
        </Form>
      </Card.Body>
    </Card>
  );
};

export default ProductForm;