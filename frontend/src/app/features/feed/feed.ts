import { ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { KudosService } from '../../core/services/kudos.service';
import { AuthService } from '../../core/auth/auth.service';
import { Category, CreateKudosRequest, KudosItem, UserListItem } from '../../core/models/kudos.model';

@Component({
  selector: 'app-feed',
  imports: [FormsModule],
  templateUrl: './feed.html',
  styleUrl: './feed.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Feed implements OnInit {
  private destroyRef = inject(DestroyRef);
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

  // AI suggest
  suggestIntent = '';
  suggestions = signal<string[]>([]);
  suggesting = signal(false);

  constructor(
    private kudosService: KudosService,
    protected auth: AuthService,
  ) {}

  ngOnInit() {
    this.loadFeed();
    this.kudosService.getCategories().pipe(takeUntilDestroyed(this.destroyRef)).subscribe(c => this.categories.set(c));
    this.kudosService.getUsers().pipe(takeUntilDestroyed(this.destroyRef)).subscribe(u => {
      const me = this.auth.currentUser()?.id;
      this.users.set(u.filter(user => user.id !== me));
    });
  }

  loadFeed() {
    this.loading.set(true);
    this.kudosService.getFeed(this.page()).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
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
    this.suggestions.set([]);
    this.suggestIntent = '';
  }

  suggestMessages() {
    const cat = this.categories().find(c => c.id === +this.categoryId);
    if (!cat || !this.suggestIntent.trim()) return;
    this.suggesting.set(true);
    this.kudosService.suggestMessage(cat.name, this.suggestIntent.trim()).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: res => {
        this.suggestions.set(res.suggestions);
        this.suggesting.set(false);
      },
      error: () => this.suggesting.set(false),
    });
  }

  useSuggestion(text: string) {
    this.message = text;
    this.suggestions.set([]);
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

    this.kudosService.create(request).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.sending.set(false);
        this.showForm.set(false);
        this.receiverId = '';
        this.categoryId = 0;
        this.message = '';
        this.suggestions.set([]);
        this.suggestIntent = '';
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
