import { ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { KudosService } from '../../core/services/kudos.service';
import { UserProfile } from '../../core/models/kudos.model';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Profile implements OnInit {
  private destroyRef = inject(DestroyRef);
  profile = signal<UserProfile | null>(null);
  loading = signal(true);
  error = signal('');

  constructor(private kudosService: KudosService) {}

  ngOnInit() {
    this.kudosService.getMyProfile().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: p => {
        this.profile.set(p);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Error loading profile');
      },
    });
  }
}
