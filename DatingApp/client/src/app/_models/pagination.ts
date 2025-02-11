export interface Pagination {
  itemsPerPage: number;
  currentPageNumber: number;
  totalPages: number;
  totalItems: number;
}

export class PaginationResult<T> {
  items?: T;
  pagination?: Pagination;
}
