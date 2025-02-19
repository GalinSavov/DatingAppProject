import { Component, inject, input, OnInit } from '@angular/core';
import { MessagesService } from '../../_services/messages.service';
import { Message } from '../../_models/message';
import { TimeAgoCustomPipe } from '../../time-ago-custom.pipe';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [TimeAgoCustomPipe],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css',
})
export class MemberMessagesComponent implements OnInit {
  ngOnInit(): void {
    this.displayMessageThread();
  }
  messagesService = inject(MessagesService);
  messages: Message[] = [];
  username = input.required<string>();

  displayMessageThread() {
    this.messagesService.getMessageThread(this.username()).subscribe({
      next: (response) => {
        this.messages = response;
      },
      error: (err) => console.log(err + ' wtf happened'),
    });
  }
}
