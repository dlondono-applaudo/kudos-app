import { Component, OnInit, signal } from '@angular/core';
import { KudosService } from '../../core/services/kudos.service';
import { UserProfile } from '../../core/models/kudos.model';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
})
export class Profile implements OnInit {
  profile = signal<UserProfile | null>(null);
  loading = signal(true);

  constructor(private kudosService: KudosService) {}

  ngOnInit() {
    this.kudosService.getMyProfile().subscribe({
      next: p => {
        this.profile.set(p);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
