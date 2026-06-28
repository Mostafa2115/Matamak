import { DecimalPipe } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { catchError, finalize, forkJoin, of } from 'rxjs';
import { ApiError } from '../../../../core/interceptors/error.interceptor';
import { MenuItem, Review } from '../../../../core/models/catalog.models';
import { CatalogService } from '../../../../core/services/catalog.service';
import { CartService } from '../../../../core/services/cart.service';

@Component({
  selector: 'app-item-detail-page',
  imports: [DecimalPipe, RouterLink],
  templateUrl: './item-detail.page.html',
  styleUrl: './item-detail.page.scss'
})
export class ItemDetailPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly catalogService = inject(CatalogService);
  private readonly cartService = inject(CartService);
  private readonly destroyRef = inject(DestroyRef);

  readonly item = signal<MenuItem | null>(null);
  readonly reviews = signal<Review[]>([]);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly quantity = signal<number>(1);
  private readonly foodImages = [
    'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=1200&q=80',
    'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?auto=format&fit=crop&w=1200&q=80',
    'https://images.unsplash.com/photo-1550547660-d9450f859349?auto=format&fit=crop&w=1200&q=80',
    'https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&w=1200&q=80',
    'https://images.unsplash.com/photo-1600891964599-f61ba0e24092?auto=format&fit=crop&w=1200&q=80',
    'https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?auto=format&fit=crop&w=1200&q=80'
  ];

  ngOnInit(): void {
    this.loadItem();
  }

  loadItem(): void {
    const itemId = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isFinite(itemId) || itemId <= 0) {
      this.isLoading.set(false);
      this.errorMessage.set('رقم الصنف غير صحيح.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    forkJoin({
      item: this.catalogService.getItem(itemId),
      reviews: this.catalogService.getReviewsForItem(itemId).pipe(catchError(() => of([])))
    })
      .pipe(
        finalize(() => this.isLoading.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: ({ item, reviews }) => {
          this.item.set(item);
          this.reviews.set(reviews);
          this.quantity.set(1);
        },
        error: (error: ApiError) => {
          this.item.set(null);
          this.reviews.set([]);
          this.errorMessage.set(error.message);
        }
      });
  }

  incrementQuantity(): void {
    this.quantity.update((q) => q + 1);
  }

  decrementQuantity(): void {
    this.quantity.update((q) => Math.max(1, q - 1));
  }

  addToCart(): void {
    const currentItem = this.item();
    if (currentItem) {
      this.cartService.addToCart(currentItem, this.quantity());
      this.cartService.openDrawer();
    }
  }

  foodImageFor(item: MenuItem): string {
    if (item.imageUrl && item.imageUrl.trim() !== '') {
      return item.imageUrl;
    }
    return this.foodImages[item.id % this.foodImages.length];
  }
}
