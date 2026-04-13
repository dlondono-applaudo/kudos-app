import { ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { KudosService } from '../../core/services/kudos.service';
import { AuthService } from '../../core/auth/auth.service';
import { UserListItem, KudosItem } from '../../core/models/kudos.model';

@Component({
  selector: 'app-admin',
  imports: [FormsModule],
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

  showCreateForm = signal(false);
  createError = signal('');
  creating = signal(false);
  newUser = { email: '', password: '', fullName: '', department: '' };

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

  toggleCreateForm() {
    this.showCreateForm.update(v => !v);
    this.createError.set('');
  }

  createUser() {
    this.creating.set(true);
    this.createError.set('');
    this.kudosService.createUser(this.newUser).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (created) => {
        this.users.update(list => [...list, { id: created.id, fullName: created.fullName, email: created.email, department: created.department }]);
        this.newUser = { email: '', password: '', fullName: '', department: '' };
        this.showCreateForm.set(false);
        this.creating.set(false);
      },
      error: (err) => {
        this.creating.set(false);
        const body = err.error;
        let msg = 'Error creating user';
        if (body?.message) {
          msg = body.message;
        } else if (body?.errors) {
          const errs = body.errors;
          msg = Array.isArray(errs) ? errs.join(', ') : Object.values(errs).flat().join(', ');
        }
        this.createError.set(msg);
      },
    });
  }
}
