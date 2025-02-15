import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { LikesService } from '../_services/likes.service';
import { Member } from '../_models/member';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { MemberCardComponent } from '../members/member-card/member-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-lists',
  standalone: true,
  imports: [FormsModule, ButtonsModule, MemberCardComponent, PaginationModule],
  templateUrl: './lists.component.html',
  styleUrl: './lists.component.css',
})
export class ListsComponent implements OnInit, OnDestroy {
  ngOnDestroy(): void {
    this.likesService.paginationResult.set(null);
  }
  ngOnInit(): void {
    this.showLikes();
  }
  likesService = inject(LikesService);

  predicate = 'liked';
  currentPageNumber = 1;
  itemsPerPage = 5;

  showLikes() {
    this.likesService.getUserLikes(
      this.predicate,
      this.currentPageNumber,
      this.itemsPerPage
    );
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
  pageChanged(event: any) {
    if (this.currentPageNumber != event.page) {
      this.currentPageNumber = event.page;
      this.showLikes();
    }
  }
}
