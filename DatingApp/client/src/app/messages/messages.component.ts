import { Component, inject, OnInit } from '@angular/core';
import { MessagesService } from '../_services/messages.service';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [],
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
}
