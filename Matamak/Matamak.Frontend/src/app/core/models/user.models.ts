import { DeliveryOrderSummary, TakeawayOrderSummary } from './order.models';

export interface AdminUser {
  username: string;
  email: string;
  phoneNumber?: string;
  address: string;
  fullName: string;
  role: 'Admin';
}

export interface CashierUser {
  username: string;
  email: string;
  address: string;
  fullName: string;
  role: 'Cashier';
}

export interface CustomerUser {
  username: string;
  email: string;
  phoneNumber: string;
  address: string;
  fullName: string;
  role: 'Customer';
  deliveryOrders: DeliveryOrderSummary[];
  takeawayOrders: TakeawayOrderSummary[];
}

