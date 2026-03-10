import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../../../core/services/user.service';
import { TrainingService } from '../../../core/services/training.service';
import { UserTrainingService } from '../../../core/services/user-training.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { forkJoin, Observable } from 'rxjs';

@Component({
  selector: 'app-user-assignments',
  templateUrl: './user-assignments.html',
  styleUrls: ['./user-assignments.css'],
  standalone: false
})
export class UserAssignmentsComponent implements OnInit {
  users: any[] = [];
  selectedUserId: number | null = null;
  
  assignedTrainings: any[] = [];
  availableTrainings: any[] = [];
  
  allTrainings: any[] = [];
  originalAssignments: any[] = [];
  
  loading: boolean = false;

  constructor(
    private userService: UserService,
    private trainingService: TrainingService,
    private userTrainingService: UserTrainingService,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    console.log(' UserAssignmentsComponent initialized');
    this.loadUsers();
    this.loadAllTrainings();
  }

  // UPDATED: Load ONLY normal users, not admins
  loadUsers(): void {
    this.userService.getNormalUsers().subscribe({
      next: (data) => {
        this.users = data;
        console.log('✅ Normal users loaded:', this.users.length);
        console.log('✅ Users list (no admins):', this.users);
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('❌ Error loading users:', error);
        this.toastr.error('Failed to load users');
      }
    });
  }

  loadAllTrainings(): void {
    this.trainingService.getAllTrainings().subscribe({
      next: (data) => {
        this.allTrainings = data;
        console.log('✅ Trainings loaded:', this.allTrainings.length);
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('❌ Error loading trainings:', error);
        this.toastr.error('Failed to load trainings');
      }
    });
  }

  // ... rest of your methods remain the same
  onUserChange(): void {
    console.log('🔵 User selected from dropdown:', this.selectedUserId);
    
    if (!this.selectedUserId) {
      this.assignedTrainings = [];
      this.availableTrainings = [];
      this.cdr.detectChanges();
      return;
    }

    this.loading = true;
    this.cdr.detectChanges();

    this.userTrainingService.getUserTrainings(this.selectedUserId).subscribe({
      next: (data) => {
        console.log('✅ User trainings loaded:', data);
        this.originalAssignments = [...data];
        this.updateTrainingLists(data);
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('❌ Error loading user trainings:', error);
        this.toastr.error('Failed to load user trainings');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  updateTrainingLists(userTrainings: any[]): void {
    const assignedIds = userTrainings.map(ut => ut.trainingId);
    console.log('🔵 Assigned training IDs:', assignedIds);
    
    this.assignedTrainings = userTrainings.map(ut => ({
      id: ut.id,
      userId: ut.userId,
      trainingId: ut.trainingId,
      trainingName: ut.trainingName || this.getTrainingName(ut.trainingId),
      status: ut.status,
      comments: ut.comments
    }));

    this.availableTrainings = this.allTrainings
      .filter(t => !assignedIds.includes(t.id))
      .map(t => ({ 
        id: t.id, 
        trainingId: t.id,
        trainingName: t.trainingName 
      }));
    
    console.log('✅ Assigned trainings:', this.assignedTrainings.length);
    console.log('✅ Available trainings:', this.availableTrainings.length);
    
    this.cdr.detectChanges();
  }

  getTrainingName(trainingId: number): string {
    const training = this.allTrainings.find(t => t.id === trainingId);
    return training ? training.trainingName : `Training ${trainingId}`;
  }

  addTraining(training: any): void {
    if (!this.selectedUserId) return;

    console.log('🔵 Adding training:', training);
    
    const newAssignment = {
      userId: this.selectedUserId,
      trainingId: training.id,
      trainingName: training.trainingName,
      status: 1,
      comments: ''
    };

    this.assignedTrainings = [...this.assignedTrainings, newAssignment];
    this.availableTrainings = this.availableTrainings.filter(t => t.id !== training.id);
    
    this.cdr.detectChanges();
  }

  removeTraining(training: any): void {
    console.log('🔵 Removing training:', training);
    
    this.availableTrainings = [...this.availableTrainings, {
      id: training.trainingId,
      trainingId: training.trainingId,
      trainingName: training.trainingName
    }];
    
    this.assignedTrainings = this.assignedTrainings.filter(t => t.trainingId !== training.trainingId);
    
    this.cdr.detectChanges();
  }

  saveAssignments(): void {
    if (!this.selectedUserId) {
      this.toastr.warning('Please select a user first', '⚠️ No User Selected');
      return;
    }

    const originalIds = this.originalAssignments.map(a => a.trainingId);
    const newIds = this.assignedTrainings.map(a => a.trainingId);

    const toRemove = originalIds.filter(id => !newIds.includes(id));
    const toAdd = newIds.filter(id => !originalIds.includes(id));

    if (toRemove.length === 0 && toAdd.length === 0) {
      this.toastr.info('No changes to save', '📝 Note');
      return;
    }

    this.loading = true;
    
    const operations: Observable<any>[] = [];

    toRemove.forEach(id => {
      operations.push(this.userTrainingService.removeTraining(this.selectedUserId!, id));
    });

    toAdd.forEach(id => {
      operations.push(this.userTrainingService.assignTraining({
        userId: this.selectedUserId!,
        trainingId: id
      }));
    });

    forkJoin(operations).subscribe({
      next: () => {
        const changes = [];
        if (toAdd.length > 0) changes.push(`Added ${toAdd.length}`);
        if (toRemove.length > 0) changes.push(`Removed ${toRemove.length}`);
        
        this.toastr.success(
          `Assignments updated successfully`,
          changes.join(' • ')
        );
        
        this.loading = false;
        this.onUserChange();
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('❌ Error updating assignments:', error);
        this.toastr.error('Failed to update assignments', '❌ Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  cancelAssignments(): void {
    if (this.selectedUserId) {
      this.updateTrainingLists(this.originalAssignments);
      this.toastr.info('Changes cancelled', '↩️ Reverted');
    }
  }

  logout(): void {
    this.authService.logout();
    this.toastr.success('Logged out successfully', '👋 Goodbye!');
    this.router.navigate(['/login']);
  }
}