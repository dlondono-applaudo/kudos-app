import { ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { KudosService } from '../../core/services/kudos.service';
import { AuthService } from '../../core/auth/auth.service';
import { UserListItem, KudosItem } from '../../core/models/kudos.model';

@Component({
  selector: 'app-admin',
  imports: [],
  templateUrl: './admin.html',
  styleUrl: './admin.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Admin implements OnInit {
  private destroyRef = inject(DestroyRef);
  users = signal<UserListItem[]>([]);
  recentKudos = signal<KudosItem[]>([]);
  loading = signal(true);
  error = signal('');

  constructor(
    private kudosService: KudosService,
    protected auth: AuthService,
  ) {}

  ngOnInit() {
    this.kudosService.getUsers().pipe(takeUntilDestroyed(this.destroyRef)).subscribe(u => this.users.set(u));
    this.kudosService.getFeed(1, 10).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: res => {
        this.recentKudos.set(res.items);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Error loading data');
      },
    });
  }

  deleteKudos(id: number) {
    this.kudosService.delete(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
      this.recentKudos.update(list => list.filter(k => k.id !== id));
    });
  }
}
