import { Component, inject, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AccountService } from '../../_services/account.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { MemberService } from '../../_services/member.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [],
  templateUrl: './edit-profile.component.html',
  styleUrl: './edit-profile.component.css',
})
export class EditProfileComponent implements OnInit {
  ngOnInit(): void {
    this.loadUserProfile();
  }
  private accountService = inject(AccountService);
  private memberService = inject(MemberService);
  member?: Member;

  loadUserProfile() {
    if (!this.accountService.currentUser()) return;
    this.memberService
      .getMember(this.accountService.currentUser()?.username)
      .subscribe({
        next: (member) => {
          this.member = member;
          console.log(member.username);
        },
        error: (error) => console.log(error),
      });
  }
}
