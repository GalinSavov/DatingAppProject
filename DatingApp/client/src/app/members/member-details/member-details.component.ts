import { Component, inject, OnInit } from '@angular/core';
import { MemberService } from '../../_services/member.service';
import { Member } from '../../_models/member';
import { ActivatedRoute } from '@angular/router';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { DatePipe } from '@angular/common';
import { TimeAgoCustomPipe } from '../../time-ago-custom.pipe';
import { MemberMessagesComponent } from "../member-messages/member-messages.component";
@Component({
  selector: 'app-member-details',
  standalone: true,
  imports: [TabsModule, GalleryModule, DatePipe, TimeAgoCustomPipe, MemberMessagesComponent],
  templateUrl: './member-details.component.html',
  styleUrl: './member-details.component.css',
})
export class MemberDetailsComponent implements OnInit {
  private memberService = inject(MemberService);
  private route = inject(ActivatedRoute);
  member?: Member;
  images: GalleryItem[] = [];

  ngOnInit(): void {
    this.displayMember();
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
