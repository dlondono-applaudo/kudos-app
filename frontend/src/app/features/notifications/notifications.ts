import { Component, OnInit, signal } from '@angular/core';
import { KudosService } from '../../core/services/kudos.service';
import { NotificationItem } from '../../core/models/kudos.model';

@Component({
  selector: 'app-notifications',
  imports: [],
  templateUrl: './notifications.html',
  styleUrl: './notifications.scss',
})
export class Notifications implements OnInit {
  items = signal<NotificationItem[]>([]);
  loading = signal(true);

  constructor(private kudosService: KudosService) {}

  ngOnInit() {
    this.kudosService.getNotifications().subscribe({
      next: data => {
        this.items.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  markRead(id: number) {
    this.kudosService.markNotificationRead(id).subscribe(() => {
      this.items.update(list =>
        list.map(n => (n.id === id ? { ...n, isRead: true } : n)),
      );
    });
  }

  markAllRead() {
    this.kudosService.markAllRead().subscribe(() => {
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
