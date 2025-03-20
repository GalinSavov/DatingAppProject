import { Component, inject, OnInit } from '@angular/core';
import { MemberService } from '../../_services/member.service';
import { MemberCardComponent } from '../member-card/member-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { AccountService } from '../../_services/account.service';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { LikesService } from '../../_services/likes.service';
@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent, PaginationModule, FormsModule, ButtonsModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  memberService = inject(MemberService);
  accountService = inject(AccountService);
  likesService = inject(LikesService);

  genderList = [
    { value: 'male', display: 'Male' },
    { value: 'female', display: 'Female' },
  ];

  ngOnInit(): void {
    let user = this.accountService.currentUser();
    if (!this.memberService.paginationResult()) this.resetFilters();
    if (user !== this.memberService.user) {
      this.resetFilters();
    }
  }
  displayMembers() {
    this.memberService.getMembers();
  }
  pageChanged(event: any) {
    if (this.memberService.userParams().currentPageNumber !== event.page) {
      this.memberService.userParams().currentPageNumber = event.page;
      this.displayMembers();
    }
  }
  resetFilters() {
    this.memberService.resetUserParams();
    this.displayMembers();
  }
}
