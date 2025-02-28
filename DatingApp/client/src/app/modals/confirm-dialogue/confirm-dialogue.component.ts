import { Component, inject } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-dialogue',
  standalone: true,
  imports: [],
  templateUrl: './confirm-dialogue.component.html',
  styleUrl: './confirm-dialogue.component.css',
})
export class ConfirmDialogueComponent {
  bsModalRef = inject(BsModalRef);
  title = '';
  message = '';
  btnOkText = '';
  btnCancelText = '';
  result = false;

  confirm() {
    this.result = true;
    this.bsModalRef.hide();
  }
  decline() {
    this.bsModalRef.hide();
  }
}
