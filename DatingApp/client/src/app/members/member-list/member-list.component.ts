import { Component, inject, OnInit } from '@angular/core';
import { MemberService } from '../../_services/member.service';
import { Member } from '../../_models/member';
import { MemberCardComponent } from '../member-card/member-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent, PaginationModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  memberService = inject(MemberService);
  currentPageNumber = 1;
  itemsPerPage = 5;

  ngOnInit(): void {
    if (!this.memberService.paginationResult()) this.displayMembers();
  }

  displayMembers() {
    this.memberService.getMembers(this.currentPageNumber, this.itemsPerPage);
  }
  pageChanged(event: any) {
    if (this.currentPageNumber !== event.page) {
      this.currentPageNumber = event.page;
      this.displayMembers();
    }
  }
}
