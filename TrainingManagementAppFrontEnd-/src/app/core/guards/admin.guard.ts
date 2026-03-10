import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    console.log('AdminGuard checking...');
    
    const isAdmin = this.authService.isAdmin();
    console.log('AdminGuard - Is admin?', isAdmin);
    
    if (isAdmin) {
      console.log('✅ AdminGuard - Access granted');
      return true;
    }
    
    console.log(' AdminGuard - Access denied, redirecting to dashboard');
    this.router.navigate(['/dashboard']);
    return false;
  }
}