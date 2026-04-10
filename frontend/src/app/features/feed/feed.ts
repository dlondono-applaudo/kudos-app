import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { KudosService } from '../../core/services/kudos.service';
import { AuthService } from '../../core/auth/auth.service';
import { Category, CreateKudosRequest, KudosItem, UserListItem } from '../../core/models/kudos.model';

@Component({
  selector: 'app-feed',
  imports: [FormsModule],
  templateUrl: './feed.html',
  styleUrl: './feed.scss',
})
export class Feed implements OnInit {
  items = signal<KudosItem[]>([]);
  categories = signal<Category[]>([]);
  users = signal<UserListItem[]>([]);
  totalCount = signal(0);
  page = signal(1);
  loading = signal(false);
  showForm = signal(false);

  receiverId = '';
  categoryId = 0;
  message = '';
  sendError = signal('');
  sending = signal(false);

  constructor(
    private kudosService: KudosService,
    protected auth: AuthService,
  ) {}

  ngOnInit() {
    this.loadFeed();
    this.kudosService.getCategories().subscribe(c => this.categories.set(c));
    this.kudosService.getUsers().subscribe(u => {
      const me = this.auth.currentUser()?.id;
      this.users.set(u.filter(user => user.id !== me));
    });
  }

  loadFeed() {
    this.loading.set(true);
    this.kudosService.getFeed(this.page()).subscribe({
      next: res => {
        this.items.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  nextPage() {
    this.page.update(p => p + 1);
    this.loadFeed();
  }

  prevPage() {
    this.page.update(p => Math.max(1, p - 1));
    this.loadFeed();
  }

  toggleForm() {
    this.showForm.update(v => !v);
    this.sendError.set('');
  }

  sendKudos() {
    if (!this.receiverId || !this.categoryId || !this.message.trim()) return;
    this.sending.set(true);
    this.sendError.set('');

    const request: CreateKudosRequest = {
      receiverId: this.receiverId,
      categoryId: this.categoryId,
      message: this.message.trim(),
    };

    this.kudosService.create(request).subscribe({
      next: () => {
        this.sending.set(false);
        this.showForm.set(false);
        this.receiverId = '';
        this.categoryId = 0;
        this.message = '';
        this.page.set(1);
        this.loadFeed();
      },
      error: err => {
        this.sending.set(false);
        this.sendError.set(err.error?.message ?? 'Failed to send kudos');
      },
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
