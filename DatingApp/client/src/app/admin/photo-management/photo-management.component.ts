import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { HttpClient } from '@microsoft/signalr';
import { Photo } from '../../_models/photo';

@Component({
  selector: 'app-photo-management',
  standalone: true,
  imports: [],
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css',
})
export class PhotoManagementComponent implements OnInit {
  adminService = inject(AdminService);
  photos: Photo[] = [];
  ngOnInit(): void {
    this.getPhotosForApproval();
  }
  getPhotosForApproval() {
    return this.adminService.getPhotosForApproval().subscribe({
      next: (response) => {
        (this.photos = response), console.log(this.photos);
      },
      error: (error) => console.log(error),
    });
  }
  approvePhoto(photoId: number) {
    return this.adminService.approvePhoto(photoId).subscribe({
      next: () => {
        let photo = this.photos.find((x) => x.id == photoId);
        if (photo != null) {
          photo.isApproved = true;
        }
      },
    });
  }
  rejectPhoto(photoId: number) {
    return this.adminService.rejectPhoto(photoId).subscribe({
      next: () => {
        this.photos = this.photos.filter((x) => x.id != photoId);
      },
    });
  }
}
