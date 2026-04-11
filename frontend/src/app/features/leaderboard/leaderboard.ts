import { ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { KudosService } from '../../core/services/kudos.service';
import { LeaderboardEntry } from '../../core/models/kudos.model';

@Component({
  selector: 'app-leaderboard',
  imports: [],
  templateUrl: './leaderboard.html',
  styleUrl: './leaderboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Leaderboard implements OnInit {
  private destroyRef = inject(DestroyRef);
  entries = signal<LeaderboardEntry[]>([]);
  loading = signal(true);
  error = signal('');

  constructor(private kudosService: KudosService) {}

  ngOnInit() {
    this.kudosService.getLeaderboard(20).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: data => {
        this.entries.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Error loading leaderboard');
      },
    });
  }

  medal(rank: number): string {
    if (rank === 1) return '🥇';
    if (rank === 2) return '🥈';
    if (rank === 3) return '🥉';
    return `#${rank}`;
  }
}
