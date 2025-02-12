import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
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
  //members = signal<Member[]>([]);
  paginationResult = signal<PaginationResult<Member[]> | null>(null);

  images: GalleryItem[] = [];

  getMember(username?: string) {
    // const member = this.members().find((x) => x.username == username);
    // if (member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  getMembers(userParams: UserParams) {
    let params = this.setHeaders(
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
          this.paginationResult.set({
            items: response.body as Member[],
            pagination: JSON.parse(response.headers.get('Pagination')!),
          });
        },
      });
  }
  private setHeaders(currentPageNumber: number, itemsPerPage: number) {
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
