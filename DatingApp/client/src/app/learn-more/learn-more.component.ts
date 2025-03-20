import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-learn-more',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './learn-more.component.html',
  styleUrl: './learn-more.component.css',
})
export class LearnMoreComponent {
  private accountService = inject(AccountService);
  ngOnInit(): void {
    this.isLoggedIn();
  }
  isLoggedIn() {
    return this.accountService.currentUser() != null;
  }
}
