import { CanDeactivateFn } from '@angular/router';
import { EditProfileComponent } from '../updates/edit-profile/edit-profile.component';
import { ConfirmService } from '../_services/confirm.service';
import { inject } from '@angular/core';

export const preventUnsavedChangesGuard: CanDeactivateFn<
  EditProfileComponent
> = (component) => {
  const confirmService = inject(ConfirmService);
  if (component.editForm?.dirty) {
    return confirmService.confirm() ?? false;
  }
  return true;
};
