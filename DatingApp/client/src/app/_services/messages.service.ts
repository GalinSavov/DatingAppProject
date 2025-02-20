import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Message } from '../_models/message';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';
import { PaginationResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class MessagesService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  paginationResult = signal<PaginationResult<Message[]> | null>(null);

  getMessage(id: number) {
    this.http.get(`${this.baseUrl}${id}`);
  }

  getMessages(
    currentPageNumber: number,
    itemsPerPage: number,
    container: string
  ) {
    let params = setPaginationHeaders(currentPageNumber, itemsPerPage);
    params = params.append('container', container);

    return this.http
      .get<Message[]>(`${this.baseUrl}messages`, {
        observe: 'response',
        params,
      })
      .subscribe({
        next: (response) =>
          setPaginatedResponse(response, this.paginationResult),
      });
  }
  getMessageThread(username: string) {
    return this.http.get<Message[]>(
      `${this.baseUrl}messages/thread/${username}`
    );
  }
  sendMessage(username: string, content: string) {
    return this.http.post<Message>(`${this.baseUrl}messages`, {
      recipientUsername: username,
      content: content,
    });
  }
  deleteMessage(id: number) {
    return this.http.delete(`${this.baseUrl}messages/${id}`);
  }
}
