import { Component, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  cancelRegister = output<boolean>(); // for child to parent communication
  private provideToastr = inject(ToastrService);
  model: any = {};
  private accountService = inject(AccountService);

  register() {
    this.accountService.register(this.model).subscribe({
      next: (response) => {
        console.log(response);
        this.cancel();
      },
      error: (error) => this.provideToastr.error(error.error),
    });
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
