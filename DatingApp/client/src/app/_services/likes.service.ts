import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  baseUrl = environment.apiUrl;
  http = inject(HttpClient);
  likeIds = signal<number[]>([]);

  toggleLike(targetId: number) {
    return this.http.post(`${this.baseUrl}likes/${targetId}`, {});
  }
  getUserLikes(predicate: string) {
    return this.http.get(`${this.baseUrl}likes?predicate=${predicate}`);
  }
  getCurrentUserLikeIds() {
    return this.http.get<number[]>(`${this.baseUrl}likes/list`).subscribe({
      next: (likeIds) => {
        this.likeIds.set(likeIds);
        console.log(`${this.baseUrl}list`);
      },
    });
  }
}
