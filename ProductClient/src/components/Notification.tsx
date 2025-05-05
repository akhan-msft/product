import { Alert } from 'react-bootstrap';
import { useEffect } from 'react';

interface NotificationProps {
  message: string;
  type: 'success' | 'danger' | 'warning' | 'info';
  onClose: () => void;
  autoDismiss?: boolean;
  dismissTimeout?: number;
}

const Notification = ({ 
  message, 
  type, 
  onClose, 
  autoDismiss = true, 
  dismissTimeout = 5000 
}: NotificationProps) => {
  
  useEffect(() => {
    if (autoDismiss && message) {
      const timer = setTimeout(() => {
        onClose();
      }, dismissTimeout);
      
      return () => clearTimeout(timer);
    }
  }, [message, autoDismiss, dismissTimeout, onClose]);
  
  if (!message) return null;
  
  return (
    <Alert 
      variant={type} 
      onClose={onClose} 
      dismissible
      className="mt-3 mb-4 shadow-sm"
    >
      {message}
    </Alert>
  );
};

export default Notification;