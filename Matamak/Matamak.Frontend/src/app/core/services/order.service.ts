import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_PATHS } from '../constants/api-paths';
import { ApiUrlService } from './api-url.service';

export interface OrderItemDto {
  name: string;
  priceForOne: number;
  quantity: number;
  note?: string;
  totalPrice: number;
}

export interface DeliveryOrderDto {
  deliveryAddress: string;
  contactNumber: string;
  customerName: string;
  totalPrice: number;
  items: OrderItemDto[];
}

export interface TakeawayOrderDto {
  customerName: string;
  totalPrice: number;
  items: OrderItemDto[];
}

export interface DineinOrderDto {
  tableNumber: number;
  totalPrice: number;
  items: OrderItemDto[];
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(ApiUrlService);

  createDeliveryOrder(order: DeliveryOrderDto): Observable<string> {
    return this.http.post(
      this.apiUrl.build(API_PATHS.orders.createDelivery),
      order,
      { responseType: 'text' }
    );
  }

  createTakeawayOrder(order: TakeawayOrderDto): Observable<string> {
    return this.http.post(
      this.apiUrl.build(API_PATHS.orders.createTakeaway),
      order,
      { responseType: 'text' }
    );
  }

  createDineinOrder(order: DineinOrderDto): Observable<string> {
    return this.http.post(
      this.apiUrl.build(API_PATHS.orders.createDinein),
      order,
      { responseType: 'text' }
    );
  }

  getDeliveryOrders(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.build(API_PATHS.orders.getDeliveryOrders));
  }

  handOrderToDriver(id: number): Observable<string> {
    return this.http.put(this.apiUrl.build(API_PATHS.orders.handOrderToDriver(id)), null, { responseType: 'text' });
  }

  handOrderToCustomer(id: number): Observable<string> {
    return this.http.put(this.apiUrl.build(API_PATHS.orders.handOrderToCustomer(id)), null, { responseType: 'text' });
  }

  cancelDeliveryOrder(id: number): Observable<string> {
    return this.http.put(this.apiUrl.build(API_PATHS.orders.cancelDeliveryOrder(id)), null, { responseType: 'text' });
  }

  getTakeawayOrders(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.build(API_PATHS.orders.getTakeawayOrders));
  }

  removeTakeawayOrder(id: number): Observable<string> {
    return this.http.delete(this.apiUrl.build(API_PATHS.orders.removeTakeawayOrder(id)), { responseType: 'text' });
  }

  changeTakeawayOrderStatus(id: number, status: string): Observable<string> {
    return this.http.put(
      this.apiUrl.build(`${API_PATHS.orders.changeTakeawayStatus(id)}?status=${encodeURIComponent(status)}`),
      null,
      { responseType: 'text' }
    );
  }

  getDineinOrders(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.build(API_PATHS.orders.getDineinOrders));
  }

  changeDineinStatus(id: number, status: string): Observable<string> {
    return this.http.put(
      this.apiUrl.build(`${API_PATHS.orders.changeDineinStatus(id)}?status=${encodeURIComponent(status)}`),
      null,
      { responseType: 'text' }
    );
  }

  removeDineinOrder(id: number): Observable<string> {
    return this.http.delete(this.apiUrl.build(API_PATHS.orders.removeDineinOrder(id)), { responseType: 'text' });
  }

  updateTakeawayOrder(id: number, order: TakeawayOrderDto): Observable<string> {
    return this.http.put(
      this.apiUrl.build(`/api/TakeAwayOrder/updateTakeAwayOrder/${id}`),
      order,
      { responseType: 'text' }
    );
  }

  updateDineinOrder(id: number, order: DineinOrderDto): Observable<string> {
    return this.http.put(
      this.apiUrl.build(`/api/DineinOrder/updateDineinOrder/${id}`),
      order,
      { responseType: 'text' }
    );
  }

  updateDeliveryOrder(id: number, order: DeliveryOrderDto): Observable<string> {
    return this.http.put(
      this.apiUrl.build(`/api/DeliveryOrder/updateDeliveryOrder/${id}`),
      order,
      { responseType: 'text' }
    );
  }


  getInventoryItems(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.build(API_PATHS.inventory.items));
  }

  getLowStockItems(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.build(API_PATHS.inventory.lowStock));
  }

  updateInventoryItem(itemId: number, quantityInStock: number, lowStockThreshold: number): Observable<any> {
    return this.http.post(
      this.apiUrl.build(API_PATHS.inventory.items),
      { itemId, quantityInStock, lowStockThreshold }
    );
  }

  getSalesReport(from?: string, to?: string): Observable<any> {
    let url = this.apiUrl.build(API_PATHS.reports.sales);
    const params: string[] = [];
    if (from) {
      params.push(`from=${encodeURIComponent(from)}`);
    }
    if (to) {
      params.push(`to=${encodeURIComponent(to)}`);
    }
    if (params.length > 0) {
      url += '?' + params.join('&');
    }
    return this.http.get<any>(url);
  }
}
