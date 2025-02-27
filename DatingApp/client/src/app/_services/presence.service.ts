import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { take } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubsUrl;
  baseUrl = environment.apiUrl;
  private hubConnection?: HubConnection;
  private toastrService = inject(ToastrService);
  onlineUsers = signal<string[]>([]);
  private router = inject(Router);

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}presence`, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));
    this.hubConnection.on('UserIsOnline', (username) => {
      this.toastrService.info(`${username} has connected!`);
    });
    this.hubConnection.on('UserIsOffline', (username) => {
      this.toastrService.warning(`${username} has disconnected!`);
    });
    this.hubConnection?.on('GetOnlineUsers', (usernames) => {
      this.onlineUsers.set(usernames);
    });
    this.hubConnection.on('NewMessageReceived!', ({ username, knownAs }) => {
      this.toastrService
        .info(`${knownAs} has sent you a new message! Click here to see`)
        .onTap.pipe(take(1))
        .subscribe({
          next: () =>
            this.router.navigateByUrl(`/members/${username}?tab=Messages`),
        });
    });
  }
  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected)
      this.hubConnection?.stop().catch((error) => console.log(error));
    else console.log('Hub connection is already stopped');
  }
}
