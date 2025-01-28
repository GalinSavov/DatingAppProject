import { Component, inject, OnInit } from '@angular/core';
import { MemberService } from '../../_services/member.service';
import { Member } from '../../_models/member';
import { ActivatedRoute } from '@angular/router';
import { TabsModule } from 'ngx-bootstrap/tabs';

@Component({
  selector: 'app-member-details',
  standalone: true,
  imports: [TabsModule],
  templateUrl: './member-details.component.html',
  styleUrl: './member-details.component.css',
})
export class MemberDetailsComponent implements OnInit {
  private memberService = inject(MemberService);
  private route = inject(ActivatedRoute);
  member?: Member;

  ngOnInit(): void {
    this.displayMember();
  }

  displayMember() {
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;

    this.memberService.getMember(username).subscribe({
      next: (response) => (this.member = response),
      error: (error) => console.log(error),
    });
  }
}
