import { Component, inject, OnInit } from '@angular/core';
import { MessagesService } from '../_services/messages.service';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { TimeAgoCustomPipe } from '../time-ago-custom.pipe';
import { Message } from '../_models/message';
import { RouterLink } from '@angular/router';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [
    ButtonsModule,
    FormsModule,
    TimeAgoCustomPipe,
    RouterLink,
    PaginationModule,
  ],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.css',
})
export class MessagesComponent implements OnInit {
  ngOnInit(): void {
    this.showMessages();
  }
  messagesService = inject(MessagesService);
  currentPageNumber = 1;
  itemsPerPage = 10;
  container = 'Inbox';
  isOutbox = this.container === 'Outbox';

  showMessages() {
    this.messagesService.getMessages(
      this.currentPageNumber,
      this.itemsPerPage,
      this.container
    );
  }
  pageChanged(event: any) {
    if (this.currentPageNumber !== event.page) {
      this.currentPageNumber = event.page;
    }
  }
  getRoute(message: Message) {
    if (this.container === 'Outbox')
      return `/members/${message.recipientUsername}`;
    else return `/members/${message.senderUsername}`;
  }
}
