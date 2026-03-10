import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { DashboardService } from '../../../core/services/dashboard.service';
import { UserTrainingService } from '../../../core/services/user-training.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-dashboard',
  templateUrl: './user-dashboard.html',
  styleUrls: ['./user-dashboard.css'],
  standalone: false
})
export class UserDashboardComponent implements OnInit {
  // Stats properties
  pendingCount: number = 0;
  inProgressCount: number = 0;
  delayedCount: number = 0;
  completedCount: number = 0;
  
  // Data properties
  trainings: any[] = [];
  userName: string = '';
  loading: boolean = true;

  // Popup properties
  showPopup: boolean = false;
  selectedTraining: any = null;
  selectedStatus: number = 1;
  expectedCompletionDate: string = '';
  comments: string = '';
  saving: boolean = false;

  // Status options for radio buttons
  statusOptions = [
    { value: 1, label: 'Pending' },
    { value: 2, label: 'In Progress' },
    { value: 3, label: 'Delayed' },
    { value: 4, label: 'Completed' }
  ];

  constructor(
    public authService: AuthService,
    private dashboardService: DashboardService,
    private userTrainingService: UserTrainingService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    console.log('🔵 Dashboard initializing...');
    
    // Get user name from auth service
    const user = this.authService.getUser();
    this.userName = user?.firstName || 'User';
    console.log('👤 Username:', this.userName);
    
    // Load dashboard data
    this.loadDashboardData();
  }
  
  // Load dashboard data from API
  loadDashboardData(): void {
    this.loading = true;
    console.log('🔵 Loading dashboard data from API...');
    
    this.cdr.detectChanges();
    
    this.dashboardService.getDashboard().subscribe({
      next: (data) => {
        console.log('✅ API Data received:', data);
        
        // Update stats
        this.pendingCount = data.pendingCount || 0;
        this.inProgressCount = data.inProgressCount || 0;
        this.delayedCount = data.delayedCount || 0;
        this.completedCount = data.completedCount || 0;
        
        // Update trainings list
        this.trainings = data.myTrainings || [];
        this.loading = false;
        
        this.cdr.detectChanges();
        
        console.log('✅ Trainings loaded:', this.trainings.length);
      },
      error: (error) => {
        console.error('❌ API Error:', error);
        this.toastr.error('Failed to load dashboard');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // Get CSS class based on status
  getStatusClass(status: number): string {
    switch(status) {
      case 1: return 'pending';
      case 2: return 'in-progress';
      case 3: return 'delayed';
      case 4: return 'completed';
      default: return '';
    }
  }

  // Get status text based on status value
  getStatusText(status: number): string {
    switch(status) {
      case 1: return 'Pending';
      case 2: return 'In Progress';
      case 3: return 'Delayed';
      case 4: return 'Completed';
      default: return 'Unknown';
    }
  }

  openStatusPopup(event: any, training: any): void {
  event.preventDefault();
  event.stopPropagation();
  
  console.log('🔵 Opening popup for:', training);
  this.selectedTraining = training;
  this.selectedStatus = training.status;
  
  // Format date if exists
  this.expectedCompletionDate = training.expectedCompletionDate ? 
    new Date(training.expectedCompletionDate).toISOString().split('T')[0] : '';
  
  this.comments = training.comments || '';
  this.showPopup = true;
  this.cdr.detectChanges();
}

  // Close popup
  closePopup(): void {
    this.showPopup = false;
    this.selectedTraining = null;
    this.expectedCompletionDate = '';
    this.comments = '';
    this.cdr.detectChanges();
  }

  // Check if save button should be disabled
  isSaveDisabled(): boolean {
    if (this.selectedStatus === 3 && !this.expectedCompletionDate) {
      return true;
    }
    return this.saving;
  }

  // Save status update
  saveStatus(): void {
    if (!this.selectedTraining) return;
    
    // Validate delayed status requires date
    if (this.selectedStatus === 3 && !this.expectedCompletionDate) {
      this.toastr.warning('Expected Completion Date is required for Delayed status');
      return;
    }
    
    this.saving = true;
    console.log('🔵 Saving status update:', {
      userTrainingId: this.selectedTraining.id,
      status: this.selectedStatus,
      expectedCompletionDate: this.expectedCompletionDate || null,
      comments: this.comments
    });

    const requestData: any = {
      userTrainingId: this.selectedTraining.id,
      status: this.selectedStatus,
      comments: this.comments
    };

    // Only add expectedCompletionDate if status is Delayed
    if (this.selectedStatus === 3 && this.expectedCompletionDate) {
      requestData.expectedCompletionDate = this.expectedCompletionDate;
    }

    this.userTrainingService.updateStatus(requestData).subscribe({
      next: (response) => {
        console.log('✅ Status update successful:', response);
        this.toastr.success('Training status updated successfully!');
        this.loadDashboardData(); // Reload fresh data
        this.closePopup();
        this.saving = false;
      },
      error: (error) => {
        console.error('❌ Status update failed:', error);
        this.toastr.error(error.error?.message || 'Failed to update status');
        this.saving = false;
        this.cdr.detectChanges();
      }
    });
  }

  // Manual refresh
  refreshDashboard(): void {
    console.log('🔄 Manual refresh triggered');
    this.loadDashboardData();
  }

  // Logout
  logout(): void {
    this.authService.logout();
    this.toastr.success('Logged out successfully');
    this.router.navigate(['/login']);
  }
}