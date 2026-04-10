import { Component, OnInit, signal } from '@angular/core';
import { KudosService } from '../../core/services/kudos.service';
import { LeaderboardEntry } from '../../core/models/kudos.model';

@Component({
  selector: 'app-leaderboard',
  imports: [],
  templateUrl: './leaderboard.html',
  styleUrl: './leaderboard.scss',
})
export class Leaderboard implements OnInit {
  entries = signal<LeaderboardEntry[]>([]);
  loading = signal(true);

  constructor(private kudosService: KudosService) {}

  ngOnInit() {
    this.kudosService.getLeaderboard(20).subscribe({
      next: data => {
        this.entries.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  medal(rank: number): string {
    if (rank === 1) return '🥇';
    if (rank === 2) return '🥈';
    if (rank === 3) return '🥉';
    return `#${rank}`;
  }
}
