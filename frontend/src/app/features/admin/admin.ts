import { Component, OnInit, signal } from '@angular/core';
import { KudosService } from '../../core/services/kudos.service';
import { AuthService } from '../../core/auth/auth.service';
import { UserListItem, KudosItem } from '../../core/models/kudos.model';

@Component({
  selector: 'app-admin',
  imports: [],
  templateUrl: './admin.html',
  styleUrl: './admin.scss',
})
export class Admin implements OnInit {
  users = signal<UserListItem[]>([]);
  recentKudos = signal<KudosItem[]>([]);
  loading = signal(true);

  constructor(
    private kudosService: KudosService,
    protected auth: AuthService,
  ) {}

  ngOnInit() {
    this.kudosService.getUsers().subscribe(u => this.users.set(u));
    this.kudosService.getFeed(1, 10).subscribe({
      next: res => {
        this.recentKudos.set(res.items);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  deleteKudos(id: number) {
    this.kudosService.delete(id).subscribe(() => {
      this.recentKudos.update(list => list.filter(k => k.id !== id));
    });
  }
}
