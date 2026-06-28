import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiUrlService {
  build(path: string): string {
    return `${environment.apiBaseUrl}${path}`;
  }
}
