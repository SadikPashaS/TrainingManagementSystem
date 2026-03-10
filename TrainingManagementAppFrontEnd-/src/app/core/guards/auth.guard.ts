import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    if (this.authService.isLoggedIn()) {
      console.log('AuthGuard: User is logged in, allowing access');
      return true;
    }
    
    console.log('AuthGuard: User not logged in, redirecting to login');
    this.router.navigate(['/login']);
    return false;
  }
}