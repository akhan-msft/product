import { useState, FormEvent } from 'react';
import { Form, Button, Row, Col, Card } from 'react-bootstrap';
import { SearchProduct } from '../types/product';

interface SearchProductsProps {
  onSearch: (searchCriteria: SearchProduct) => void;
}

const SearchProducts = ({ onSearch }: SearchProductsProps) => {
  const [searchCriteria, setSearchCriteria] = useState<SearchProduct>({
    query: '',
    category: ''
  });

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    onSearch(searchCriteria);
  };

  const handleInputChange = (name: keyof SearchProduct, value: string | undefined) => {
    setSearchCriteria(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const clearForm = () => {
    setSearchCriteria({
      query: '',
      category: ''
    });
  };

  return (
    <Card className="mb-4">
      <Card.Header>
        <h4>Search Products</h4>
      </Card.Header>
      <Card.Body>
        <Form onSubmit={handleSubmit}>
          <Row className="mb-3">
            <Col md={6}>
              <Form.Group>
                <Form.Label>Search Query</Form.Label>
                <Form.Control
                  type="text"
                  placeholder="Search by name or description"
                  value={searchCriteria.query || ''}
                  onChange={(e) => handleInputChange('query', e.target.value)}
                />
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group>
                <Form.Label>Category</Form.Label>
                <Form.Control
                  type="text"
                  placeholder="Filter by category"
                  value={searchCriteria.category || ''}
                  onChange={(e) => handleInputChange('category', e.target.value)}
                />
              </Form.Group>
            </Col>
          </Row>

          <div className="d-flex gap-2">
            <Button variant="primary" type="submit">
              Search
            </Button>
            <Button variant="secondary" type="button" onClick={clearForm}>
              Clear
            </Button>
          </div>
        </Form>
      </Card.Body>
    </Card>
  );
};

export default SearchProducts;