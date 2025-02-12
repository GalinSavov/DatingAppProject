import { Component, inject, OnInit } from '@angular/core';
import { MemberService } from '../../_services/member.service';
import { Member } from '../../_models/member';
import { MemberCardComponent } from '../member-card/member-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { UserParams } from '../../_models/userParams';
import { AccountService } from '../../_services/account.service';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

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
  userParams = new UserParams(this.accountService.currentUser());
  genderList = [
    { value: 'male', display: 'Male' },
    { value: 'female', display: 'Female' },
  ];

  ngOnInit(): void {
    if (!this.memberService.paginationResult()) this.displayMembers();
  }

  displayMembers() {
    this.memberService.getMembers(this.userParams);
  }
  pageChanged(event: any) {
    if (this.userParams.currentPageNumber !== event.page) {
      this.userParams.currentPageNumber = event.page;
      this.displayMembers();
    }
  }
  resetFilters() {
    this.userParams = new UserParams(this.accountService.currentUser());
    this.displayMembers();
  }
}
