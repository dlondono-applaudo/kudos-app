export interface KudosItem {
  id: number;
  senderName: string;
  senderDepartment: string;
  receiverName: string;
  receiverDepartment: string;
  categoryName: string;
  message: string;
  points: number;
  sentimentEmoji: string | null;
  createdAt: string;
}

export interface KudosFeedResponse {
  items: KudosItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface CreateKudosRequest {
  receiverId: string;
  categoryId: number;
  message: string;
}

export interface Category {
  id: number;
  name: string;
  description: string;
  pointValue: number;
}

export interface LeaderboardEntry {
  userId: string;
  fullName: string;
  department: string;
  totalPoints: number;
  kudosReceived: number;
  rank: number;
}

export interface NotificationItem {
  id: number;
  message: string;
  isRead: boolean;
  createdAt: string;
}

export interface UserProfile {
  id: string;
  fullName: string;
  email: string;
  department: string;
  totalPointsReceived: number;
  kudosSent: number;
  kudosReceived: number;
  badges: Badge[];
}

export interface Badge {
  name: string;
  description: string;
  icon: string;
  awardedAt: string;
}

export interface UserListItem {
  id: string;
  fullName: string;
  email: string;
  department: string;
}
