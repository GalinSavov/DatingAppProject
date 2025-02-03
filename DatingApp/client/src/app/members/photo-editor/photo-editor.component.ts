import {
  Component,
  inject,
  input,
  OnInit,
  output,
  ChangeDetectorRef,
} from '@angular/core';
import { Member } from '../../_models/member';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { MemberService } from '../../_services/member.service';
import { Photo } from '../../_models/photo';

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [NgClass, NgFor, NgIf, NgStyle, FileUploadModule, DecimalPipe],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css',
})
export class PhotoEditorComponent implements OnInit {
  ngOnInit(): void {
    this.initializeUploader();
  }
  member = input.required<Member>();
  memberChange = output<Member>();
  private accountService = inject(AccountService);
  private memberService = inject(MemberService);
  baseUrl = environment.apiUrl;
  hasBaseDropZoneOver = false;
  uploader?: FileUploader;

  fileOverBase(event: any) {
    this.hasBaseDropZoneOver = event;
  }
  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo).subscribe({
      next: (_) => {
        const user = this.accountService.currentUser();
        if (user) {
          user.photoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }
        const updatedMember = { ...this.member() };
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.url == photo.url) p.isMain = true;
        });
        this.memberChange.emit(updatedMember);
      },
    });
  }
  deletePhoto(photo: Photo) {
    this.memberService.deletePhoto(photo).subscribe({
      next: (_) => {
        const updatedMember = { ...this.member() };
        updatedMember.photos = updatedMember.photos.filter(
          (x) => x.id !== photo.id
        );
        this.memberChange.emit(updatedMember);
      },
      error: (error) => console.log(error),
    });
  }
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.accountService.currentUser()?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      const photo = JSON.parse(response);
      const updatedMember = { ...this.member() };
      updatedMember.photos.push(photo);
      this.memberChange.emit(updatedMember);
    };
  }
}
