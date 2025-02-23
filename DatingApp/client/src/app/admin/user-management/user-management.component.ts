import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { User } from '../../_models/user';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css',
})
export class UserManagementComponent implements OnInit {
  adminService = inject(AdminService);
  users: User[] = [];
  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe({
      next: (response) => (this.users = response),
    });
  }
  ngOnInit(): void {
    this.getUsersWithRoles();
  }
}
