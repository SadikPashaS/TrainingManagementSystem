import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css'],
  standalone: false
})
export class NavbarComponent implements OnInit {
  userName: string = '';
  showDropdown: boolean = false;
  isAdminUser: boolean = false;
  currentUrl: string = '';

  constructor(
    public authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {
    // Track current URL
    this.router.events.subscribe(() => {
      this.currentUrl = this.router.url;
    });
  }

  ngOnInit(): void {
    const user = this.authService.getUser();
    if (user) {
      this.userName = user.firstName || user.userName || 'User';
      this.isAdminUser = this.authService.isAdmin();
    }
    this.currentUrl = this.router.url;
  }

  // Check if current page is Training Management
  isTrainingManagementPage(): boolean {
    return this.currentUrl.includes('/admin/trainings');
  }

  // Check if current page is User Overview
  isUserOverviewPage(): boolean {
    return this.currentUrl.includes('/admin/users');
  }

  // Check if current page is Assignments
  isAssignmentsPage(): boolean {
    return this.currentUrl.includes('/admin/assignments');
  }

  toggleMenu(): void {
    this.showDropdown = !this.showDropdown;
  }

  closeMenu(): void {
    this.showDropdown = false;
  }

  logout(): void {
    this.authService.logout();
    this.toastr.success('Logged out successfully');
    this.router.navigate(['/login']);
  }
  // Add this method to check if any option is visible
showAnyOption(): boolean {
  return !this.isTrainingManagementPage() || 
         !this.isUserOverviewPage() || 
         !this.isAssignmentsPage();
}
}