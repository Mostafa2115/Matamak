import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DecimalPipe } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { catchError, debounceTime, distinctUntilChanged, finalize, forkJoin, of } from 'rxjs';
import { ApiError } from '../../../../core/interceptors/error.interceptor';
import { Category, Country, MenuItem } from '../../../../core/models/catalog.models';
import { CatalogService } from '../../../../core/services/catalog.service';
import { CartService } from '../../../../core/services/cart.service';

@Component({
  selector: 'app-menu-page',
  imports: [DecimalPipe, ReactiveFormsModule, RouterLink],
  templateUrl: './menu.page.html',
  styleUrl: './menu.page.scss'
})
export class MenuPage implements OnInit {
  private readonly catalogService = inject(CatalogService);
  readonly cartService = inject(CartService);
  private readonly destroyRef = inject(DestroyRef);

  readonly searchControl = new FormControl('', { nonNullable: true });
  readonly categories = signal<Category[]>([]);
  readonly countries = signal<Country[]>([]);
  readonly items = signal<MenuItem[]>([]);
  readonly selectedCategoryId = signal<number | null>(null);
  readonly selectedCountryId = signal<number | null>(null);
  readonly isLoadingLookups = signal(true);
  readonly isLoadingItems = signal(true);
  readonly errorMessage = signal<string | null>(null);
  private readonly foodImages = [
    'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=900&q=80',
    'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?auto=format&fit=crop&w=900&q=80',
    'https://images.unsplash.com/photo-1550547660-d9450f859349?auto=format&fit=crop&w=900&q=80',
    'https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&w=900&q=80',
    'https://images.unsplash.com/photo-1600891964599-f61ba0e24092?auto=format&fit=crop&w=900&q=80',
    'https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?auto=format&fit=crop&w=900&q=80'
  ];

  ngOnInit(): void {
    this.loadLookups();
    this.loadItems();

    this.searchControl.valueChanges
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.loadItems());
  }

  selectCategory(categoryId: number | null): void {
    this.selectedCategoryId.set(categoryId);
    this.loadItems();
  }

  selectCountry(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.selectedCountryId.set(value ? Number(value) : null);
    this.loadItems();
  }

  clearFilters(): void {
    this.selectedCategoryId.set(null);
    this.selectedCountryId.set(null);
    this.searchControl.setValue('');
    this.loadItems();
  }

  trackById(_: number, item: { id: number }): number {
    return item.id;
  }

  private loadLookups(): void {
    this.isLoadingLookups.set(true);
    forkJoin({
      categories: this.catalogService.getCategories().pipe(catchError(() => of([]))),
      countries: this.catalogService.getCountries().pipe(catchError(() => of([])))
    })
      .pipe(
        finalize(() => this.isLoadingLookups.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(({ categories, countries }) => {
        this.categories.set(categories);
        this.countries.set(countries);
      });
  }

  loadItems(): void {
    this.isLoadingItems.set(true);
    this.errorMessage.set(null);

    this.catalogService
      .getItems({
        term: this.searchControl.value,
        categoryId: this.selectedCategoryId(),
        countryId: this.selectedCountryId()
      })
      .pipe(
        finalize(() => this.isLoadingItems.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: (items) => this.items.set(items),
        error: (error: ApiError) => {
          this.items.set([]);
          this.errorMessage.set(error.message);
        }
      });
  }

  addToCart(item: MenuItem): void {
    this.cartService.addToCart(item, 1);
    this.cartService.openDrawer();
  }

  foodImageFor(item: MenuItem, index: number): string {
    if (item.imageUrl && item.imageUrl.trim() !== '') {
      return item.imageUrl;
    }
    return this.foodImages[(item.id + index) % this.foodImages.length];
  }
}
