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
  ngOnInit(): void {}
  messagesService = inject(MessagesService);
  messages = input.required<Message[]>();
  username = input.required<string>();
}
