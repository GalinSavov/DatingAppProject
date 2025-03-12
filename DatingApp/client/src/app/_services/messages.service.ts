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
import { group } from '@angular/animations';
import { Group } from '../_models/group';
import { BusyService } from './busy.service';

@Injectable({
  providedIn: 'root',
})
export class MessagesService {
  private http = inject(HttpClient);
  private busyService = inject(BusyService);
  baseUrl = environment.apiUrl;
  hubConnection?: HubConnection;
  paginationResult = signal<PaginationResult<Message[]> | null>(null);
  hubUrl = environment.hubsUrl;
  messageThread = signal<Message[]>([]);
  createHubConnection(user: User, otherUser: string) {
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}message?user=${otherUser}`, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();
    this.hubConnection
      .start()
      .catch((error) => console.log(error))
      .finally(() => this.busyService.idle());
    this.hubConnection.on('ReceiveMessageThread', (messages) => {
      this.messageThread.set(messages.result);
    });
    this.hubConnection.on('NewMessage', (newMessage) => {
      this.messageThread.update((messages) => [...messages, newMessage]);
      console.log(newMessage);
    });
    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if (group.connections.some((x) => x.username === otherUser)) {
        this.messageThread.update((messages) => {
          messages.forEach((message) => {
            if (!message.dateRead) {
              message.dateRead = new Date(Date.now());
            }
          });
          return messages;
        });
      }
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
  async sendMessage(username: string, content: string) {
    return this.hubConnection?.invoke('SendMessage', {
      recipientUsername: username,
      content,
    });
  }
  deleteMessage(id: number) {
    return this.http.delete(`${this.baseUrl}messages/${id}`);
  }
}
