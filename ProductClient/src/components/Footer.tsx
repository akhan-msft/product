import React from 'react';
import { Container, Row, Col } from 'react-bootstrap';
import copilotLogo from '../assets/copilot.png';

const Footer: React.FC = () => {
  return (
    <footer className="mt-3 py-2 border-top">
      <Container>
        <Row className="justify-content-center align-items-center">
          <Col xs="auto" className="d-flex align-items-center">
            <span className="text-muted me-2">Powered by GitHub Copilot</span>
            <img 
              src={copilotLogo} 
              alt="GitHub Copilot" 
              style={{ height: '30px', width: 'auto' }} 
            />
          </Col>
        </Row>
      </Container>
    </footer>
  );
};

export default Footer;