import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { User } from '../../_models/user';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css',
})
export class UserManagementComponent implements OnInit {
  modalService = inject(BsModalService);
  adminService = inject(AdminService);
  users: User[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> =
    new BsModalRef<RolesModalComponent>();

  openRolesModal() {
    const initialState: ModalOptions = {
      class: 'modal-lg',
      initialState: {
        title: 'User roles',
        list: ['Admin', 'Moderator', 'Member'],
      },
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, initialState);
  }
  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe({
      next: (response) => (this.users = response),
    });
  }
  ngOnInit(): void {
    this.getUsersWithRoles();
  }
}
