import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of, throwError } from 'rxjs';
import { API_PATHS } from '../constants/api-paths';
import { Category, Country, MenuFilters, MenuItem, Review } from '../models/catalog.models';
import { ApiUrlService } from './api-url.service';

@Injectable({ providedIn: 'root' })
export class CatalogService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(ApiUrlService);

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl.build(API_PATHS.catalog.categories));
  }

  getCountries(): Observable<Country[]> {
    return this.http.get<Country[]>(this.apiUrl.build(API_PATHS.catalog.countries));
  }

  getItems(filters: MenuFilters = {}): Observable<MenuItem[]> {
    const term = filters.term?.trim();
    if (term) {
      return this.searchItems(term);
    }

    if (filters.categoryId || filters.countryId) {
      let params = new HttpParams();
      if (filters.countryId) {
        params = params.set('countryId', filters.countryId);
      }
      if (filters.categoryId) {
        params = params.set('categoryId', filters.categoryId);
      }

      return this.http
        .get<MenuItem[]>(this.apiUrl.build(API_PATHS.catalog.sortItems), { params })
        .pipe(catchError((error) => this.mapEmptyNotFound<MenuItem>(error)));
    }

    return this.http
      .get<MenuItem[]>(this.apiUrl.build(API_PATHS.catalog.items))
      .pipe(catchError((error) => this.mapEmptyNotFound<MenuItem>(error)));
  }

  getItem(id: number): Observable<MenuItem> {
    return this.http.get<MenuItem>(this.apiUrl.build(API_PATHS.catalog.itemById(id)));
  }

  getReviewsForItem(itemId: number): Observable<Review[]> {
    return this.http.get<Review[]>(this.apiUrl.build(API_PATHS.catalog.reviewsByItem(itemId)));
  }

  addItem(item: Omit<MenuItem, 'id'>): Observable<string> {
    return this.http.post(this.apiUrl.build(API_PATHS.catalog.addItem), item, { responseType: 'text' });
  }

  updateItem(id: number, item: Omit<MenuItem, 'id'>): Observable<string> {
    return this.http.put(this.apiUrl.build(API_PATHS.catalog.updateItem(id)), item, { responseType: 'text' });
  }

  removeItem(id: number): Observable<string> {
    return this.http.delete(this.apiUrl.build(API_PATHS.catalog.removeItem(id)), { responseType: 'text' });
  }

  uploadItemImage(file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string }>(this.apiUrl.build('/api/items/uploadImage'), formData);
  }

  addCategory(name: string): Observable<string> {
    return this.http.post(this.apiUrl.build(API_PATHS.catalog.addCategory), { name }, { responseType: 'text' });
  }

  removeCategory(id: number): Observable<string> {
    return this.http.delete(this.apiUrl.build(`${API_PATHS.catalog.removeCategory}?id=${id}`), { responseType: 'text' });
  }

  editCategory(id: number, name: string): Observable<string> {
    return this.http.put(this.apiUrl.build(`${API_PATHS.catalog.editCategory}?id=${id}`), { name }, { responseType: 'text' });
  }

  addCountry(name: string): Observable<string> {
    return this.http.post(this.apiUrl.build(API_PATHS.catalog.addCountry), { name }, { responseType: 'text' });
  }

  removeCountry(id: number): Observable<string> {
    return this.http.delete(this.apiUrl.build(`${API_PATHS.catalog.removeCountry}?id=${id}`), { responseType: 'text' });
  }

  editCountry(id: number, name: string): Observable<string> {
    return this.http.put(this.apiUrl.build(`${API_PATHS.catalog.editCountry}?id=${id}`), { name }, { responseType: 'text' });
  }

  private searchItems(term: string): Observable<MenuItem[]> {
    return this.http.get<MenuItem[]>(this.apiUrl.build(API_PATHS.catalog.search), {
      params: new HttpParams().set('term', term)
    });
  }

  private mapEmptyNotFound<T>(error: HttpErrorResponse): Observable<T[]> {
    if (error.status === 404) {
      return of([]);
    }

    return throwError(() => error);
  }
}
