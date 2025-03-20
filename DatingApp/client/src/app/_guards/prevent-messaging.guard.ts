import { CanActivateFn } from '@angular/router';
import { MemberService } from '../_services/member.service';
import { inject } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { LikesService } from '../_services/likes.service';

import { map, switchMap, tap } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

export const preventMessagingGuard: CanActivateFn = (route, state) => {
  const memberService = inject(MemberService);
  const accountService = inject(AccountService);
  const likesService = inject(LikesService);
  const targetUsername = route.paramMap.get('username');
  const toastrService = inject(ToastrService);
  const tab = route.queryParamMap.get('tab');
  if (!targetUsername) {
    console.log('Could not find username in param map');
    return false;
  }
  const sourceUser = accountService.currentUser();
  if (!sourceUser) {
    console.log('User not logged in!');
    return false;
  }
  if (tab !== 'Messages') {
    return true;
  }
  return memberService.getMember(targetUsername).pipe(
    switchMap((targetUser) => {
      return likesService.mutualLike(sourceUser.username, targetUser.username);
    }),
    map((hasMutualLike) => {
      if (!hasMutualLike) {
        toastrService.error(
          `The messages can be accessed only after ${targetUsername} has liked you back!`
        );
      }
      return hasMutualLike;
    })
  );
};
