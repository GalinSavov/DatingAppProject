import {
  AfterViewChecked,
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
export class MemberMessagesComponent implements AfterViewChecked {
  @ViewChild('messageForm') messageForm?: NgForm;
  @ViewChild('scrollMe') scrollContainer?: any;
  messagesService = inject(MessagesService);
  username = input.required<string>();
  messageContent = '';
  loading = false;
  sendMessage() {
    this.loading = true;
    this.messagesService
      .sendMessage(this.username(), this.messageContent)
      .then(() => {
        this.messageForm?.reset();
        this.scrollToBottom();
      })
      .finally(() => (this.loading = false))
      .catch((error) => {
        if (error) {
          console.log(error);
        } else {
          console.log('No error found in SignalR');
        }
      });
  }
  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }
  private scrollToBottom() {
    if (this.scrollContainer) {
      this.scrollContainer.nativeElement.scrollTop =
        this.scrollContainer.nativeElement.scrollHeight;
    }
  }
}
