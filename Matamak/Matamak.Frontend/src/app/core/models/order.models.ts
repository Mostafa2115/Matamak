import { MenuItem } from './catalog.models';

export interface DeliveryOrderSummary {
  id: number;
  orderNumber: number;
  orderDate?: string;
  totalPrice?: number;
  status?: string;
}

export interface TakeawayOrderSummary {
  id: number;
  orderNumber: number;
  orderDate?: string;
  totalPrice?: number;
  status?: string;
  customerName?: string;
}

export interface OrderHistoryItem {
  type: 'Delivery' | 'Takeaway';
  id: number;
  orderNumber: number;
  orderDate: string;
  totalPrice: number;
  status: string;
}

export interface CartItem {
  item: MenuItem;
  quantity: number;
  note?: string;
}

