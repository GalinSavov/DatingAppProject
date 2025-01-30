import {
  Component,
  HostListener,
  inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import { AccountService } from '../../_services/account.service';
import { Member } from '../../_models/member';
import { MemberService } from '../../_services/member.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditorComponent } from '../../members/photo-editor/photo-editor.component';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [TabsModule, FormsModule, PhotoEditorComponent],
  templateUrl: './edit-profile.component.html',
  styleUrl: './edit-profile.component.css',
})
export class EditProfileComponent implements OnInit {
  @ViewChild('editForm') editForm?: NgForm; //accesses the form in the HTML
  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
    // a browser guard against unsaved changes
    if (this.editForm?.dirty) $event.returnValue = true;
  }
  ngOnInit(): void {
    this.loadUserProfile();
  }
  private accountService = inject(AccountService);
  private memberService = inject(MemberService);
  private toastr = inject(ToastrService);
  member?: Member;

  loadUserProfile() {
    if (!this.accountService.currentUser()) return;
    this.memberService
      .getMember(this.accountService.currentUser()?.username)
      .subscribe({
        next: (member) => {
          this.member = member;
          console.log(member.username);
        },
        error: (error) => console.log(error),
      });
  }
  updateMember() {
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: (_) => {
        this.toastr.success('Updated profile successfully!');
        this.editForm?.reset(this.member);
      },
    });
  }
  onMemberChanged(event: Member) {
    this.member = event;
  }
}
