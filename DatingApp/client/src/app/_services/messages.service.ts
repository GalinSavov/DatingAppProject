import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Message } from '../_models/message';
import { setPaginatedResponse, setPaginationHeaders } from './paginationHelper';
import { PaginationResult } from '../_models/pagination';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root',
})
export class MessagesService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  hubConnection?: HubConnection;
  paginationResult = signal<PaginationResult<Message[]> | null>(null);
  hubUrl = environment.hubsUrl;
  messageThread = signal<Message[]>([]);
  createHubConnection(user: User, otherUser: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}message?user=${otherUser}`, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();
    this.hubConnection.start().catch((error) => console.log(error));
    this.hubConnection.on('ReceiveMessageThread', (messages) => {
      this.messageThread.set(messages.result);
    });
  }
  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected)
      this.hubConnection?.stop().catch((error) => console.log(error));
    else {
      console.log('SignalR message hub is already stopped');
    }
  }

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
