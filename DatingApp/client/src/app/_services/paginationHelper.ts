import { HttpParams, HttpResponse } from '@angular/common/http';
import { Member } from '../_models/member';
import { Pagination, PaginationResult } from '../_models/pagination';
import { signal } from '@angular/core';

export function setPaginatedResponse<T>(
  response: HttpResponse<T>,
  paginationResultSignal: ReturnType<typeof signal<PaginationResult<T> | null>>
) {
  paginationResultSignal.set({
    items: response.body as T,
    pagination: JSON.parse(response.headers.get('Pagination')!),
  });
}
export function setPaginationHeaders(
  currentPageNumber: number,
  itemsPerPage: number
) {
  let params = new HttpParams();
  if (currentPageNumber && itemsPerPage) {
    params = params.append('currentPageNumber', currentPageNumber);
    params = params.append('itemsPerPage', itemsPerPage);
  }

  return params;
}
