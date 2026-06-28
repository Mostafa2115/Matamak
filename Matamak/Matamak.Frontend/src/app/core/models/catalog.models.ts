export interface Category {
  id: number;
  name: string;
}

export interface Country {
  id: number;
  name: string;
}

export interface MenuItem {
  id: number;
  name: string | null;
  description: string | null;
  price: number;
  imageUrl: string | null;
  catogryId: number;
  countryId: number;
  isAvailable: boolean;
}

export interface Review {
  id: number;
  itemId: number;
  itemName: string;
  customerUsername: string;
  rating: number;
  comment: string | null;
  createdAt: string;
}

export interface MenuFilters {
  term?: string;
  categoryId?: number | null;
  countryId?: number | null;
}
