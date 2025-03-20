import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HasRoleDirective } from '../_directives/has-role.directive';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    BsDropdownModule,
    RouterLink,
    RouterLinkActive,
    HasRoleDirective,
  ],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
  accountService = inject(AccountService);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  model: any = {};

  login() {
    this.accountService.login(this.model).subscribe({
      next: (_) => {
        this.router.navigateByUrl('/members');
      },
      error: (error) => this.toastr.error(error.error),
    });
  }
  logout() {
    this.accountService.logout();
  }
}
