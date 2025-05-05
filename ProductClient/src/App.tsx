import { useState, useEffect } from 'react';
import { Container, Row, Col, Tabs, Tab } from 'react-bootstrap';
import SearchProducts from './components/SearchProducts';
import ProductList from './components/ProductList';
import ProductForm from './components/ProductForm';
import Notification from './components/Notification';
import Footer from './components/Footer';
import { ProductService } from './services/productService';
import { Product, SearchProduct, AddProduct } from './types/product';
import CloneTrooperHelmet from './assets/images/clone-trooper-helmet.png';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

function App() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [notification, setNotification] = useState<{ message: string; type: 'success' | 'danger' | 'warning' | 'info' } | null>(null);
  const [activeTab, setActiveTab] = useState<string>('search');

  // Load products on first render
  useEffect(() => {
    fetchAllProducts();
  }, []);

  const fetchAllProducts = async () => {
    setLoading(true);
    try {
      const fetchedProducts = await ProductService.getAllProducts();
      setProducts(fetchedProducts);
    } catch (error) {
      showNotification('Failed to fetch products. Please try again.', 'danger');
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async (searchCriteria: SearchProduct) => {
    setLoading(true);
    try {
      const searchResults = await ProductService.searchProducts(searchCriteria);
      setProducts(searchResults);
      if (searchResults.length === 0) {
        showNotification('No products found matching your search criteria.', 'info');
      }
    } catch (error) {
      showNotification('An error occurred while searching. Please try again.', 'danger');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateProduct = async (product: AddProduct) => {
    setLoading(true);
    try {
      await ProductService.createProduct(product);
      showNotification('Product created successfully!', 'success');
      
      // Refresh products list and switch to search tab
      await fetchAllProducts();
      setActiveTab('search');
    } catch (error) {
      showNotification('Failed to create product. Please try again.', 'danger');
    } finally {
      setLoading(false);
    }
  };

  const showNotification = (message: string, type: 'success' | 'danger' | 'warning' | 'info') => {
    setNotification({ message, type });
  };

  const clearNotification = () => {
    setNotification(null);
  };

  return (
    <>
      <Container className="py-4">
        <div className="star-wars-title-container mb-4">
          <div className="title-with-image">
            <div className="helmet-image-container">
              <img src={CloneTrooperHelmet} alt="Clone Trooper Helmet" className="helmet-image" />
            </div>
            <div className="title-text">
              <h1 className="text-center star-wars-title">Republic Inventory</h1>
              <h5 className="text-center star-wars-subtitle">Clone Trooper Division Asset Management</h5>
            </div>
          </div>
        </div>
        
        {notification && (
          <Notification 
            message={notification.message} 
            type={notification.type} 
            onClose={clearNotification} 
          />
        )}
        
        <Tabs
          activeKey={activeTab}
          onSelect={(k) => setActiveTab(k || 'search')}
          className="mb-4"
        >
          <Tab eventKey="search" title="Search Products">
            <Row>
              <Col lg={12}>
                <SearchProducts onSearch={handleSearch} />
              </Col>
            </Row>
            <Row>
              <Col lg={12}>
                <ProductList products={products} isLoading={loading} />
              </Col>
            </Row>
          </Tab>
          <Tab eventKey="create" title="Create Product">
            <Row>
              <Col lg={12}>
                <ProductForm 
                  onSubmit={handleCreateProduct} 
                  isLoading={loading}
                />
              </Col>
            </Row>
          </Tab>
        </Tabs>
      </Container>
      <Footer />
    </>
  );
}

export default App;
