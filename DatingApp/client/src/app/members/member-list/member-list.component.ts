import { Component, inject, OnInit } from '@angular/core';
import { MemberService } from '../../_services/member.service';
import { Member } from '../../_models/member';
import { MemberCardComponent } from "../member-card/member-card.component";

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  ngOnInit(): void {
    this.displayMembers();
  }
  private memberService = inject(MemberService);
  members: Member[] = [];
  displayMembers() {
    this.memberService.getMembers().subscribe({
      next: (response) => (this.members = response),
      error: (error) => console.log(error),
    });
  }
}
