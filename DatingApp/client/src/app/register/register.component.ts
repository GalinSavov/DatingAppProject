import { Component, inject, OnInit, output } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { JsonPipe, NgIf } from '@angular/common';
import { TextInputComponent } from '../_forms/text-input/text-input.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, JsonPipe, TextInputComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  cancelRegister = output<boolean>(); // for child to parent communication
  private provideToastr = inject(ToastrService);
  model: any = {};
  private accountService = inject(AccountService);
  registerForm: FormGroup = new FormGroup({});
  private formBuilder = inject(FormBuilder);

  ngOnInit(): void {
    this.initializeForm();
  }
  initializeForm() {
    this.registerForm = this.formBuilder.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
      gender: ['male'],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () =>
        this.registerForm.controls['confirmPassword'].updateValueAndValidity(),
    });
  }
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : { isMatching: true };
    };
  }

  register() {
    console.log(this.registerForm.value);
    // this.accountService.register(this.model).subscribe({
    //   next: (response) => {
    //     console.log(response);
    //     this.cancel();
    //   },
    //   error: (error) => this.provideToastr.error(error.error),
    // });
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
