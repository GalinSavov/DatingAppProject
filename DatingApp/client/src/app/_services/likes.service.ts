import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Member } from '../_models/member';
import { PaginationResult } from '../_models/pagination';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  baseUrl = environment.apiUrl;
  http = inject(HttpClient);
  likeIds = signal<number[]>([]);
  paginationResult = signal<PaginationResult<Member[]> | null>(null);

  toggleLike(targetId: number) {
    return this.http.post(`${this.baseUrl}likes/${targetId}`, {});
  }
  getUserLikes(
    predicate: string,
    currentPageNumber: number,
    itemsPerPage: number
  ) {
    let params = setPaginationHeaders(currentPageNumber, itemsPerPage);
    params = params.append('predicate', predicate);
    return this.http
      .get<Member[]>(`${this.baseUrl}likes`, {
        observe: 'response',
        params,
      })
      .subscribe({
        next: (response) =>
          setPaginatedResponse(response, this.paginationResult),
      });
  }
  getCurrentUserLikeIds() {
    return this.http.get<number[]>(`${this.baseUrl}likes/list`).subscribe({
      next: (likeIds) => {
        this.likeIds.set(likeIds);
        console.log(`${this.baseUrl}list`);
      },
    });
  }
  mutualLike(sourceUsername: string, targetUsername: string) {
    let params = new HttpParams()
      .set('sourceUsername', sourceUsername)
      .set('targetUsername', targetUsername);
    return this.http.get<boolean>(`${this.baseUrl}likes/has-mutual-like`, {
      params,
    });
  }
}
