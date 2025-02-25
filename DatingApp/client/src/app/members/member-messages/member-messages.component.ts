import {
  Component,
  inject,
  input,
  OnInit,
  output,
  ViewChild,
} from '@angular/core';
import { MessagesService } from '../../_services/messages.service';
import { Message } from '../../_models/message';
import { TimeAgoCustomPipe } from '../../time-ago-custom.pipe';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [TimeAgoCustomPipe, FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css',
})
export class MemberMessagesComponent {
  @ViewChild('messageForm') messageForm?: NgForm;
  messagesService = inject(MessagesService);
  username = input.required<string>();
  messageContent = '';
  sendMessage() {
    this.messagesService
      .sendMessage(this.username(), this.messageContent)
      .subscribe({
        next: (message) => {
          this.messageForm?.reset();
        },
      });
  }
}
