import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Interest } from '../_models/interest';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class InterestsService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  allInterests = signal<Interest[]>([]);
  interestsForCurrentUser = signal<Interest[]>([]);
  interestNames = signal<string[]>([]);
  getInterests() {
    return this.http.get<Interest[]>(`${this.baseUrl}interests`).subscribe({
      next: (interests) => this.allInterests.set(interests),
    });
  }
  getInterestsForUser(username: string) {
    return this.http
      .get<Interest[]>(`${this.baseUrl}interests/${username}`)
      .subscribe({
        next: (interests) => {
          this.interestsForCurrentUser.set(interests);
          this.interestNames.set(interests.map((x) => x.name));
        },
      });
  }
  deleteInterestFromUser(username: string, interestId: number) {
    return this.http.delete(
      `${this.baseUrl}interests/${username}/${interestId}`
    );
  }
  addInterestToUser(username: string, interestId: number) {
    return this.http.post(
      `${this.baseUrl}interests/${username}/${interestId}`,
      []
    );
  }
}
