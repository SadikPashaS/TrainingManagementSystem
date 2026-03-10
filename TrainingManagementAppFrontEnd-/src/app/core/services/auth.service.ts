import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

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
  role: number | string;
  token: string;
  tokenExpiry: Date;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  private tokenKey = 'auth_token';
  private userKey = 'user_data';

  constructor(private http: HttpClient) {
    console.log('AuthService initialized with API URL:', this.apiUrl);
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    console.log('🔵 Login API call started with:', credentials.userId);
    
    return this.http.post<LoginResponse>(`${this.apiUrl}/Auth/login`, credentials).pipe(
      tap({
        next: (response) => {
          console.log('✅ Login API success:', response);
          console.log('User role:', response.role, 'Type:', typeof response.role);
          
          localStorage.setItem(this.tokenKey, response.token);
          localStorage.setItem(this.userKey, JSON.stringify(response));
          
          console.log('✅ Token and user data stored');
        },
        error: (error) => {
          console.error('❌ Login API error in tap:', error);
        }
      }),
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    console.error('🔴 handleError called with:', error);
    
    let errorMessage = 'An unknown error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
      console.error('Client-side error:', error.error.message);
    } else {
      // Server-side error
      console.error('Server-side error:', {
        status: error.status,
        message: error.message,
        error: error.error
      });
      
      if (error.status === 401) {
        errorMessage = 'Invalid User ID or Password';
      } else if (error.status === 400) {
        errorMessage = 'Invalid request. Please check your input.';
      } else if (error.status === 0) {
        errorMessage = 'Cannot connect to server. Please check if backend is running.';
      } else {
        errorMessage = error.error?.message || `Error ${error.status}: ${error.message}`;
      }
    }
    
    return throwError(() => ({
      status: error.status,
      message: errorMessage,
      error: error.error
    }));
  }

  logout(): void {
    console.log('Logging out, clearing localStorage');
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
  }

  getToken(): string | null {
    const token = localStorage.getItem(this.tokenKey);
    return token;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getUser(): LoginResponse | null {
    const user = localStorage.getItem(this.userKey);
    if (user) {
      try {
        const parsed = JSON.parse(user);
        return parsed;
      } catch (e) {
        console.error('Error parsing user data:', e);
        return null;
      }
    }
    return null;
  }

  getUserName(): string {
    const user = this.getUser();
    if (!user) return '';
    return user.firstName ? `${user.firstName} ${user.lastName || ''}`.trim() : user.userName;
  }

  isAdmin(): boolean {
    const user = this.getUser();
    if (!user) return false;
    
    if (typeof user.role === 'number') {
      return user.role === 2;
    }
    return user.role === 'Admin';
  }
}