export interface User {
  id: number;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  isActive: boolean;
  createdAt: Date;
  lastLoginAt?: Date;
}

export enum UserRole {
  Admin = 'Admin',
  NormalUser = 'NormalUser'
}

export interface LoginRequest {
  userId: string;
  password: string;
}

export interface LoginResponse {
  userId: number;
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  token: string;
  tokenExpiry: Date;
}