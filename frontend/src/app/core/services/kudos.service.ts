import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  KudosFeedResponse,
  CreateKudosRequest,
  Category,
  LeaderboardEntry,
  NotificationItem,
  UserProfile,
  UserListItem,
  KudosItem,
} from '../models/kudos.model';

@Injectable({ providedIn: 'root' })
export class KudosService {
  private api = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getFeed(page = 1, pageSize = 20) {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<KudosFeedResponse>(`${this.api}/kudos`, { params });
  }

  create(request: CreateKudosRequest) {
    return this.http.post<KudosItem>(`${this.api}/kudos`, request);
  }

  delete(id: number) {
    return this.http.delete(`${this.api}/kudos/${id}`);
  }

  getCategories() {
    return this.http.get<Category[]>(`${this.api}/categories`);
  }

  getLeaderboard(top = 10) {
    const params = new HttpParams().set('top', top);
    return this.http.get<LeaderboardEntry[]>(`${this.api}/leaderboard`, { params });
  }

  getNotifications() {
    return this.http.get<NotificationItem[]>(`${this.api}/notifications`);
  }

  getUnreadCount() {
    return this.http.get<number>(`${this.api}/notifications/unread-count`);
  }

  markNotificationRead(id: number) {
    return this.http.post(`${this.api}/notifications/${id}/read`, {});
  }

  markAllRead() {
    return this.http.post(`${this.api}/notifications/read-all`, {});
  }

  getMyProfile() {
    return this.http.get<UserProfile>(`${this.api}/users/me`);
  }

  getUserProfile(id: string) {
    return this.http.get<UserProfile>(`${this.api}/users/${id}`);
  }

  getUsers() {
    return this.http.get<UserListItem[]>(`${this.api}/users`);
  }
}
