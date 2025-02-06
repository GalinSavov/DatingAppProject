import { NgIf } from '@angular/common';
import { Component, input, Self } from '@angular/core';
import {
  ControlValueAccessor,
  FormControl,
  NgControl,
  ReactiveFormsModule,
} from '@angular/forms';
import {
  BsDatepickerModule,
  BsDatepickerConfig,
} from 'ngx-bootstrap/datepicker';
@Component({
  selector: 'app-date-picker',
  standalone: true,
  imports: [BsDatepickerModule, NgIf, ReactiveFormsModule],
  templateUrl: './date-picker.component.html',
  styleUrl: './date-picker.component.css',
})
export class DatePickerComponent implements ControlValueAccessor {
  label = input<string>();
  maxDate = input<Date>();
  bootstrapConfig?: Partial<BsDatepickerConfig>;
  constructor(@Self() public NgControl: NgControl) {
    this.NgControl.valueAccessor = this;
    this.bootstrapConfig = {
      containerClass: 'theme-blue',
      dateInputFormat: 'DD MMMM YYYY',
    };
  }
  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}
  get control(): FormControl {
    return this.NgControl.control as FormControl;
  }
}
