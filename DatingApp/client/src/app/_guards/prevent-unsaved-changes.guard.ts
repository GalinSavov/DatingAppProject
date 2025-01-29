import { CanDeactivateFn } from '@angular/router';
import { EditProfileComponent } from '../updates/edit-profile/edit-profile.component';

export const preventUnsavedChangesGuard: CanDeactivateFn<
  EditProfileComponent
> = (component) => {
  if (component.editForm?.dirty) {
    return confirm(
      'Are you sure you want to continue? Any unsaved changes will be lost!'
    );
  }
  return true;
};
