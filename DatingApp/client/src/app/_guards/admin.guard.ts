import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

export const adminGuard: CanActivateFn = (route, state) => {
  const accoutService = inject(AccountService);
  const toastr = inject(ToastrService);

  if (
    accoutService.roles().includes('Admin') ||
    accoutService.roles().includes('Moderator')
  ) {
    return true;
  } else {
    toastr.error('Not authorised to access this page!');
    return false;
  }
};
