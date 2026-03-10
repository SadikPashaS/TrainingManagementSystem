import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface User {
  id: number;
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  role: number | string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Get ALL users (for admin purposes)
  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/Users`);
  }

  // Get ONLY normal users (Role = 1 or "User")
  getNormalUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/Users`).pipe(
      map(users => users.filter(user => {
        // Handle both number role (1) and string role ("User")
        if (typeof user.role === 'number') {
          return user.role === 1;  // 1 = NormalUser in your enum
        } else {
          return user.role === 'User' || user.role === 'NormalUser';
        }
      }))
    );
  }

  getUserById(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/Users/${id}`);
  }
}