import {
  Component,
  computed,
  effect,
  inject,
  input,
  OnInit,
  signal,
} from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../_services/likes.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
})
export class MemberCardComponent implements OnInit {
  ngOnInit(): void {
    this.getMutualLike();
  }
  member = input.required<Member>();
  hasMutualLike = signal<boolean>(false); //signal
  likesService = inject(LikesService);
  private presenceService = inject(PresenceService);
  private accountService = inject(AccountService);
  hasLiked = computed(() =>
    this.likesService.likeIds().includes(this.member().id)
  );
  isOnline = computed(() =>
    this.presenceService.onlineUsers().includes(this.member().username)
  );

  toggleLike() {
    this.likesService.toggleLike(this.member().id).subscribe({
      next: () => {
        if (this.hasLiked()) {
          this.likesService.likeIds.update((ids) =>
            ids.filter((id) => id !== this.member().id)
          );
          this.getMutualLike();
        } else {
          this.likesService.likeIds.update((ids) => [...ids, this.member().id]);
          this.getMutualLike();
        }
      },
    });
  }
  getMutualLike() {
    return this.likesService
      .mutualLike(
        this.accountService.currentUser()?.username!,
        this.member().username
      )
      .subscribe({
        next: (response) => this.hasMutualLike.set(response),
      });
  }
}
