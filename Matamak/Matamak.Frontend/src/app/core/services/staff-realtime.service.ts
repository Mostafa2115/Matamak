import { Injectable, NgZone, inject, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenStorageService } from './token-storage.service';

@Injectable({ providedIn: 'root' })
export class StaffRealtimeService {
  private readonly tokenStorage = inject(TokenStorageService);
  private readonly zone = inject(NgZone);
  private connection: signalR.HubConnection | null = null;

  readonly isConnected = signal(false);
  readonly newDeliveryOrder$ = new Subject<any>();
  readonly deliveryOrderStatusChanged$ = new Subject<{ order: any; message?: string }>();
  readonly newTakeawayOrder$ = new Subject<any>();
  readonly takeawayOrderRemoved$ = new Subject<any>();
  readonly takeawayOrderStatusChanged$ = new Subject<{ order: any; message?: string }>();

  async start(): Promise<void> {
    if (this.connection) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRHubUrl, {
        accessTokenFactory: () => this.tokenStorage.getAccessToken() || '',
        withCredentials: false
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.connection.on('ReceiveNewOrder', (order: any) => {
      this.zone.run(() => this.newDeliveryOrder$.next(order));
    });

    this.connection.on('ReceiveOrderStatusChanged', (order: any, message?: string) => {
      this.zone.run(() => this.deliveryOrderStatusChanged$.next({ order, message }));
    });

    this.connection.on('ReceiveNewTakeawayOrder', (order: any) => {
      this.zone.run(() => this.newTakeawayOrder$.next(order));
    });

    this.connection.on('ReceiveTakeawayOrderRemoved', (order: any) => {
      this.zone.run(() => this.takeawayOrderRemoved$.next(order));
    });

    this.connection.on('ReceiveTakeawayOrderStatusChanged', (order: any, message?: string) => {
      this.zone.run(() => this.takeawayOrderStatusChanged$.next({ order, message }));
    });

    this.connection.onreconnected(() => this.zone.run(() => this.isConnected.set(true)));
    this.connection.onreconnecting(() => this.zone.run(() => this.isConnected.set(false)));
    this.connection.onclose(() => this.zone.run(() => this.isConnected.set(false)));

    try {
      await this.connection.start();
      this.zone.run(() => this.isConnected.set(true));
    } catch {
      this.zone.run(() => this.isConnected.set(false));
      this.connection = null;
    }
  }

  async stop(): Promise<void> {
    if (!this.connection) {
      return;
    }

    const connection = this.connection;
    this.connection = null;
    connection.off('ReceiveNewOrder');
    connection.off('ReceiveOrderStatusChanged');
    connection.off('ReceiveNewTakeawayOrder');
    connection.off('ReceiveTakeawayOrderRemoved');
    connection.off('ReceiveTakeawayOrderStatusChanged');
    await connection.stop();
    this.isConnected.set(false);
  }
}
