import { Component, inject, OnInit } from '@angular/core';
import { LikesService } from '../_services/likes.service';
import { Member } from '../_models/member';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { MemberCardComponent } from '../members/member-card/member-card.component';

@Component({
  selector: 'app-lists',
  standalone: true,
  imports: [FormsModule, ButtonsModule, MemberCardComponent],
  templateUrl: './lists.component.html',
  styleUrl: './lists.component.css',
})
export class ListsComponent implements OnInit {
  ngOnInit(): void {
    this.showLikes();
  }
  private likesService = inject(LikesService);
  members: Member[] = [];
  predicate = 'liked';

  showLikes() {
    this.likesService.getUserLikes(this.predicate).subscribe({
      next: (members) => (this.members = members),
    });
  }
  getTitle(): string {
    switch (this.predicate) {
      case 'liked':
        return 'Users You Liked';
      case 'likedBy':
        return 'Users Who Liked You';
      default:
        return 'Mutual';
    }
  }
}
