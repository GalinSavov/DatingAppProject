import {
  HttpClient,
  HttpHeaders,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { GalleryItem, ImageItem } from 'ng-gallery';
import { Photo } from '../_models/photo';
import { PaginationResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  paginationResult = signal<PaginationResult<Member[]> | null>(null);
  images: GalleryItem[] = [];
  memberCache = new Map();

  getMember(username?: string) {
    // const member = this.members().find((x) => x.username == username);
    // if (member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  getMembers(userParams: UserParams) {
    const cachedParams = this.memberCache.get(
      Object.values(userParams).join('-')
    );

    if (cachedParams) {
      return this.setPaginatedResponse(cachedParams);
    }

    let params = this.setPaginationHeaders(
      userParams.currentPageNumber,
      userParams.itemsPerPage
    );

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.http
      .get<Member[]>(this.baseUrl + 'users/', { observe: 'response', params })
      .subscribe({
        next: (response) => {
          this.setPaginatedResponse(response);
          this.memberCache.set(Object.values(userParams).join('-'), response);
        },
      });
  }
  private setPaginatedResponse(response: HttpResponse<Member[]>) {
    this.paginationResult.set({
      items: response.body as Member[],
      pagination: JSON.parse(response.headers.get('Pagination')!),
    });
  }
  private setPaginationHeaders(
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
  updateMember(member: Member) {
    return this.http
      .put(this.baseUrl + 'users', member)
      .pipe
      // tap(() => {
      //   this.members.update((members) =>
      //     members.map((m) => (m.username === member.username ? member : m))
      //   );
      // })
      ();
  }
  setMainPhoto(photo: Photo) {
    return this.http
      .put(this.baseUrl + 'users/set-main-photo/' + photo.id, {})
      .pipe
      // tap(() => {
      //   this.members.update((members) =>
      //     members.map((m) => {
      //       if (m.photos.includes(photo)) {
      //         m.photoUrl = photo.url;
      //       }
      //       return m;
      //     })
      //   );
      // })
      ();
  }
  deletePhoto(photo: Photo) {
    return this.http
      .delete(this.baseUrl + 'users/delete-photo/' + photo.id)
      .pipe
      // tap(() => {
      //   this.members.update((members) =>
      //     members.map((m) => {
      //       if (m.photos.includes(photo)) {
      //         m.photos = m.photos.filter((x) => x.id !== photo.id);
      //       }
      //       return m;
      //     })
      //   );
      // })
      ();
  }
}
