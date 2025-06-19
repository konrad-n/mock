export interface SignInRequest {
  email: string;
  password: string;
}

export interface SignUpRequest {
  email: string;
  password: string;
  fullName: string;
  smkVersion: 'old' | 'new';
}

export interface AuthResponse {
  AccessToken: string;
  RefreshToken: string;
  Expires: number;
  UserId: number;
  Role: string;
  Claims: Record<string, string[]>;
}