export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  fullName: string;
  department: string;
}

export interface AuthResponse {
  token: string;
  expiration: string;
  email: string;
  fullName: string;
}
