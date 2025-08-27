import { Component, inject, OnInit } from '@angular/core';
import { MemberService } from '../../_services/member.service';
import { MemberCardComponent } from '../member-card/member-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { AccountService } from '../../_services/account.service';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { LikesService } from '../../_services/likes.service';
import { InterestsService } from '../../_services/interests.service';
import { MultiSelectModule } from '@syncfusion/ej2-angular-dropdowns';
@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [
    MemberCardComponent,
    PaginationModule,
    FormsModule,
    ButtonsModule,
    MultiSelectModule,
  ],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  memberService = inject(MemberService);
  accountService = inject(AccountService);
  likesService = inject(LikesService);
  interestsService = inject(InterestsService);
  dropdownOpen = false;
  genderList = [
    { value: 'male', display: 'Male' },
    { value: 'female', display: 'Female' },
  ];
  ngOnInit(): void {
    let user = this.accountService.currentUser();
    if (!this.memberService.paginationResult()) this.resetFilters();
    if (user?.username !== this.memberService.user?.username) {
      this.resetFilters();
    }
    this.interestsService.getInterests();
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
  toggleInterest(interest: string) {
    const index = this.memberService.userParams().interests.indexOf(interest);
    if (index > -1) {
      this.memberService.userParams().interests.splice(index, 1);
    } else {
      this.memberService.userParams().interests.push(interest);
    }
    console.log(this.memberService.userParams().interests);
  }
  toggleDropdown() {
    this.dropdownOpen = !this.dropdownOpen; // Open/close the dropdown
  }
}
