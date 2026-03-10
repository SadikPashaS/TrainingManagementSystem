import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { UserTrainingService } from '../../../core/services/user-training.service';
import { UserService } from '../../../core/services/user.service';
import { ToastrService } from 'ngx-toastr';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-user-overview',
  templateUrl: './user-overview.html',
  styleUrls: ['./user-overview.css'],
  standalone: false
})
export class UserOverviewComponent implements OnInit {
  userTrainings: any[] = [];
  users: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  constructor(
    private userTrainingService: UserTrainingService,
    private userService: UserService,
    private toastr: ToastrService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    // Listen for navigation events to reload data
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      if (event.url === '/admin/users') {
        console.log('🔄 Navigated to User Overview, reloading data...');
        this.loadUsers();
      }
    });
  }

  ngOnInit(): void {
    console.log('🔵 UserOverviewComponent initialized');
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
        console.log('✅ Users loaded:', this.users);
        this.loadUserTrainings();
      },
      error: (error) => {
        console.error('❌ Error loading users:', error);
        this.loadUserTrainings();
      }
    });
  }

  loadUserTrainings(): void {
    this.loading = true;
    this.errorMessage = '';
    console.log('🔵 Loading user trainings from API...');
    
    this.cdr.detectChanges();
    
    this.userTrainingService.getAllUserTrainings().subscribe({
      next: (data) => {
        console.log('✅ API Data received:', data);
        this.userTrainings = data || [];
        this.loading = false;
        this.cdr.detectChanges();
        
        if (this.userTrainings.length === 0) {
          console.log('⚠️ No user trainings found');
        }
      },
      error: (error) => {
        console.error('❌ API Error:', error);
        this.errorMessage = 'Failed to load user trainings';
        this.toastr.error(this.errorMessage);
        this.loading = false;
        this.userTrainings = [];
        this.cdr.detectChanges();
      }
    });
  }

  getUserDisplayName(item: any): string {
    // METHOD 1: Check if userName is provided and not empty
    if (item.userName && item.userName.trim() !== '' && item.userName !== 'User 2') {
      return item.userName;
    }
    
    // METHOD 2: Find the user in our users list using userId
    if (this.users && this.users.length > 0) {
      const foundUser = this.users.find(u => u.id === item.userId);
      if (foundUser) {
        return `${foundUser.firstName || ''} ${foundUser.lastName || ''}`.trim() || foundUser.userId;
      }
    }
    
    // METHOD 3: Fallback to "User [ID]"
    return `User ${item.userId}`;
  }

  getStatusClass(status: number): string {
    switch(status) {
      case 1: return 'status-pending';
      case 2: return 'status-inprogress';
      case 3: return 'status-delayed';
      case 4: return 'status-completed';
      default: return '';
    }
  }

  getStatusText(status: number): string {
    switch(status) {
      case 1: return 'Pending';
      case 2: return 'In Progress';
      case 3: return 'Delayed';
      case 4: return 'Completed';
      default: return 'Unknown';
    }
  }
}