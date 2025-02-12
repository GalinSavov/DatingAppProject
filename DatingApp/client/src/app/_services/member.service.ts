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
import { of } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  baseUrl = environment.apiUrl;
  paginationResult = signal<PaginationResult<Member[]> | null>(null);
  images: GalleryItem[] = [];
  memberCache = new Map();
  user = this.accountService.currentUser();
  userParams = signal<UserParams>(new UserParams(this.user));
  resetUserParams() {
    this.userParams.set(new UserParams(this.user));
  }

  getMember(username?: string) {
    const member: Member = [...this.memberCache.values()]
      .reduce((arr, element) => arr.concat(element.body), [])
      .find((x: Member) => x.username == username);
    console.log(member);

    if (member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  getMembers() {
    const cachedParams = this.memberCache.get(
      Object.values(this.userParams()).join('-')
    );

    if (cachedParams) {
      return this.setPaginatedResponse(cachedParams);
    }

    let params = this.setPaginationHeaders(
      this.userParams().currentPageNumber,
      this.userParams().itemsPerPage
    );

    params = params.append('minAge', this.userParams().minAge);
    params = params.append('maxAge', this.userParams().maxAge);
    params = params.append('gender', this.userParams().gender);
    params = params.append('orderBy', this.userParams().orderBy);

    return this.http
      .get<Member[]>(this.baseUrl + 'users/', { observe: 'response', params })
      .subscribe({
        next: (response) => {
          this.setPaginatedResponse(response);
          this.memberCache.set(
            Object.values(this.userParams()).join('-'),
            response
          );
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
