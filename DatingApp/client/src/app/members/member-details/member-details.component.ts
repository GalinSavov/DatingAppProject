import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { MemberService } from '../../_services/member.service';
import { Member } from '../../_models/member';
import { ActivatedRoute } from '@angular/router';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { DatePipe } from '@angular/common';
import { TimeAgoCustomPipe } from '../../time-ago-custom.pipe';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/message';
import { MessagesService } from '../../_services/messages.service';
@Component({
  selector: 'app-member-details',
  standalone: true,
  imports: [
    TabsModule,
    GalleryModule,
    DatePipe,
    TimeAgoCustomPipe,
    MemberMessagesComponent,
  ],
  templateUrl: './member-details.component.html',
  styleUrl: './member-details.component.css',
})
export class MemberDetailsComponent implements OnInit {
  @ViewChild('memberTabs') memberTabs?: TabsetComponent;
  activeTab?: TabDirective;
  private memberService = inject(MemberService);
  private route = inject(ActivatedRoute);
  member?: Member;
  images: GalleryItem[] = [];
  messages: Message[] = [];
  private messagesService = inject(MessagesService);

  ngOnInit(): void {
    this.displayMember();
  }
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (
      this.activeTab.heading === 'Messages' &&
      this.messages.length === 0 &&
      this.member
    ) {
      this.messagesService.getMessageThread(this.member.username).subscribe({
        next: (response) => {
          this.messages = response;
        },
      });
    }
  }

  displayMember() {
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;

    this.memberService.getMember(username).subscribe({
      next: (response) => {
        this.member = response;
        this.member.photos.map((photo) => {
          this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
        });
      },
      error: (error) => console.log(error),
    });
  }
}
