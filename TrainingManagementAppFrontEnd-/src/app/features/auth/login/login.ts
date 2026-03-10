import { Component, ChangeDetectorRef } from '@angular/core'; // Add ChangeDetectorRef
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
  standalone: false
})
export class LoginComponent {
  userId = '';
  password = '';
  loading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef  // Add this
  ) {}

  onSubmit(): void {
    // Clear previous error
    this.errorMessage = '';
    
    // Validate inputs
    if (!this.userId || !this.password) {
      this.toastr.warning('Please enter both User ID and Password');
      return;
    }
    
    // Set loading state
    this.loading = true;
    console.log('🔵 Login attempt with:', { userId: this.userId });
    
    this.authService.login({ userId: this.userId, password: this.password }).subscribe({
      next: (response) => {
        console.log('✅ Login successful:', response);
        this.toastr.success('Login successful!');
        this.loading = false;
        this.cdr.detectChanges(); // Force change detection
        
        if (this.authService.isAdmin()) {
          this.router.navigate(['/admin/trainings']);
        } else {
          this.router.navigate(['/dashboard']);
        }
      },
      error: (error) => {
        console.error('❌ Login error in component:', error);
        
        this.loading = false;
        this.errorMessage = error.message || 'Login failed. Please try again.';
        // this.toastr.error(this.errorMessage);
        
        // Force change detection to update the view
        this.cdr.detectChanges();
        
        console.log('🔵 Loading set to false, error displayed:', this.errorMessage);
      }
    });
  }
}