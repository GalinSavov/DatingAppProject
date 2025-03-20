import {
  Component,
  computed,
  inject,
  OnDestroy,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { Member } from '../../_models/member';
import { ActivatedRoute, Router } from '@angular/router';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { DatePipe } from '@angular/common';
import { TimeAgoCustomPipe } from '../../time-ago-custom.pipe';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessagesService } from '../../_services/messages.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';
import { HubConnectionState } from '@microsoft/signalr';
import { LikesService } from '../../_services/likes.service';
import { ToastrService } from 'ngx-toastr';
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
export class MemberDetailsComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  activeTab?: TabDirective;
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  private messagesService = inject(MessagesService);
  presenceService = inject(PresenceService);
  private accountService = inject(AccountService);
  private toastrService = inject(ToastrService);
  likesService = inject(LikesService);
  hasMutualLike = signal<boolean>(false);
  hasLiked = computed(() =>
    this.likesService.likeIds().includes(this.member.id)
  );

  ngOnInit(): void {
    this.getMemberFromRouteResolver();
    this.route.queryParams.subscribe({
      next: (params) => {
        params['tab'] && this.selectTab(params['tab']);
      },
    });
    this.route.paramMap.subscribe({
      next: () => this.onRouteParamsChanged(),
    });
    this.getMutualLike();
  }
  ngOnDestroy(): void {
    this.messagesService.stopHubConnection();
  }
  getMemberFromRouteResolver() {
    this.route.data.subscribe({
      next: (data) => {
        this.member = data['member'];
        this.member &&
          this.member.photos.map((photo) => {
            this.images.push(
              new ImageItem({ src: photo.url, thumb: photo.url })
            );
          });
      },
    });
  }
  selectTab(heading: string) {
    if (this.memberTabs) {
      const messageTab = this.memberTabs.tabs.find((x) => x.heading == heading);
      if (messageTab) messageTab.active = true;
    }
  }
  onRouteParamsChanged() {
    const user = this.accountService.currentUser();
    if (!user) return;
    if (
      this.messagesService.hubConnection?.state ===
        HubConnectionState.Connected &&
      this.activeTab?.heading === 'Messages'
    ) {
      this.messagesService.hubConnection.stop().then(() => {
        this.messagesService.createHubConnection(user, this.member.username);
      });
    }
  }
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { tab: this.activeTab.heading },
      queryParamsHandling: 'merge',
    });
    if (this.activeTab.heading === 'Messages' && this.member) {
      this.getMessageThread();
    } else {
      this.messagesService.stopHubConnection();
    }
  }
  getMessageThread() {
    const user = this.accountService.currentUser();
    if (user === null) return;
    this.messagesService.createHubConnection(user, this.member.username);
  }
  toggleLike() {
    this.likesService.toggleLike(this.member.id).subscribe({
      next: () => {
        if (this.hasLiked()) {
          this.likesService.likeIds.update((ids) =>
            ids.filter((id) => id !== this.member.id)
          );
          this.getMutualLike();
        } else {
          this.likesService.likeIds.update((ids) => [...ids, this.member.id]);
          this.getMutualLike();
        }
      },
    });
  }
  getMutualLike() {
    return this.likesService
      .mutualLike(
        this.accountService.currentUser()?.username!,
        this.member.username
      )
      .subscribe({
        next: (response) => this.hasMutualLike.set(response),
      });
  }
  messageAccessWarning(event: Event, username: string) {
    const messageTab = event.target as HTMLElement;
    console.log('HELLO');

    if (messageTab.hasAttribute('disabled')) {
      console.log('Clicked!');
      this.toastrService.info(
        `You can not message ${username} until they have liked you back!`
      );
    }
  }
}
