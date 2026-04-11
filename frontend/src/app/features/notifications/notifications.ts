import { ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { KudosService } from '../../core/services/kudos.service';
import { NotificationItem } from '../../core/models/kudos.model';

@Component({
  selector: 'app-notifications',
  imports: [],
  templateUrl: './notifications.html',
  styleUrl: './notifications.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Notifications implements OnInit {
  private destroyRef = inject(DestroyRef);
  items = signal<NotificationItem[]>([]);
  loading = signal(true);
  error = signal('');

  constructor(private kudosService: KudosService) {}

  ngOnInit() {
    this.kudosService.getNotifications().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: data => {
        this.items.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Error loading notifications');
      },
    });
  }

  markRead(id: number) {
    this.kudosService.markNotificationRead(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
      this.items.update(list =>
        list.map(n => (n.id === id ? { ...n, isRead: true } : n)),
      );
    });
  }

  markAllRead() {
    this.kudosService.markAllRead().pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
      this.items.update(list => list.map(n => ({ ...n, isRead: true })));
    });
  }

  timeAgo(dateStr: string): string {
    const diff = Date.now() - new Date(dateStr).getTime();
    const mins = Math.floor(diff / 60000);
    if (mins < 1) return 'just now';
    if (mins < 60) return `${mins}m ago`;
    const hours = Math.floor(mins / 60);
    if (hours < 24) return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    return `${days}d ago`;
  }
}
