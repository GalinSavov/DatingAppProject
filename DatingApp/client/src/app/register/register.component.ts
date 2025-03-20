import { Component, inject, OnInit, output } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { TextInputComponent } from '../_forms/text-input/text-input.component';
import { DatePickerComponent } from '../_forms/date-picker/date-picker.component';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    TextInputComponent,
    DatePickerComponent,
    RouterLink,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  private accountService = inject(AccountService);
  registerForm: FormGroup = new FormGroup({});
  private formBuilder = inject(FormBuilder);
  private router = inject(Router);
  maxDate = new Date();
  validationErrors: string[] = [];

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
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
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
    console.log(dob);
    this.registerForm.patchValue({ dateOfBirth: dob });
    this.accountService.register(this.registerForm.value).subscribe({
      next: () => {
        console.log('Hello!!!');
        this.router.navigateByUrl('/members');
      },
      error: (error) => (this.validationErrors = error),
    });
  }
  private getDateOnly(dateOfBirth: string | undefined) {
    if (!dateOfBirth) return;
    return new Date(dateOfBirth).toISOString().slice(0, 10);
  }
}
