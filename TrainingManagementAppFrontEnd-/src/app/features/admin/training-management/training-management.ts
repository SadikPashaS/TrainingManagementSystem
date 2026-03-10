import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { TrainingService } from '../../../core/services/training.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-training-management',
  templateUrl: './training-management.html',
  styleUrls: ['./training-management.css'],
  standalone: false
})
export class TrainingManagementComponent implements OnInit {
  trainings: any[] = [];
  showModal = false;
  editMode = false;
  selectedTrainingId: number | null = null;

  formData = {
    trainingName: '',
    trainingUrl: '',
    mode: 2,
    platform: '',
    expectedStartDate: '',
    expectedEndDate: ''
  };

  constructor(
    private trainingService: TrainingService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadTrainings();
  }

  loadTrainings(): void {
    this.trainingService.getAllTrainings().subscribe({
      next: (data) => {
        this.trainings = data;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error loading trainings:', error);
        this.toastr.error('Failed to load trainings');
      }
    });
  }

  getModeClass(mode: number): string {
    switch(mode) {
      case 1: return 'badge-instructor';
      case 2: return 'badge-virtual';
      default: return '';
    }
  }

  getModeText(mode: number): string {
    switch(mode) {
      case 1: return 'Instructor-Led';
      case 2: return 'Virtual';
      default: return 'Unknown';
    }
  }

  openAddModal(): void {
    this.editMode = false;
    this.selectedTrainingId = null;
    this.formData = {
      trainingName: '',
      trainingUrl: '',
      mode: 2,
      platform: '',
      expectedStartDate: '',
      expectedEndDate: ''
    };
    this.showModal = true;
  }

  openEditModal(training: any): void {
    this.editMode = true;
    this.selectedTrainingId = training.id;
    this.formData = {
      trainingName: training.trainingName,
      trainingUrl: training.trainingUrl,
      mode: training.mode,
      platform: training.platform,
      expectedStartDate: training.expectedStartDate ? new Date(training.expectedStartDate).toISOString().split('T')[0] : '',
      expectedEndDate: training.expectedEndDate ? new Date(training.expectedEndDate).toISOString().split('T')[0] : ''
    };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
  }

  // UPDATED saveTraining method with date validation
  saveTraining(): void {
    console.log('🔵 SaveTraining called with data:', this.formData);
    
    // ===== NEW DATE VALIDATION =====
    // Check if end date is before start date
    if (this.formData.expectedStartDate && this.formData.expectedEndDate) {
      const startDate = new Date(this.formData.expectedStartDate);
      const endDate = new Date(this.formData.expectedEndDate);
      
      if (endDate < startDate) {
        console.warn('⚠️ End date cannot be before start date');
        this.toastr.warning('End date must be after start date', 'Invalid Dates');
        return; // Stop the save process
      }
    }
    
    // Validate form data
    if (!this.formData.trainingName || !this.formData.trainingUrl || !this.formData.platform || 
        !this.formData.expectedStartDate || !this.formData.expectedEndDate) {
      console.warn('⚠️ Validation failed - missing fields');
      this.toastr.warning('Please fill in all fields');
      return;
    }

    // Format the data properly for backend
    const requestData = {
      trainingName: this.formData.trainingName,
      trainingUrl: this.formData.trainingUrl,
      mode: Number(this.formData.mode),
      platform: this.formData.platform,
      expectedStartDate: new Date(this.formData.expectedStartDate).toISOString(),
      expectedEndDate: new Date(this.formData.expectedEndDate).toISOString()
    };

    console.log('🔵 Sending formatted data:', requestData);

    if (this.editMode && this.selectedTrainingId) {
      // Update existing training
      const updateData = {
        id: this.selectedTrainingId,
        ...requestData,
        isActive: true
      };
      
      this.trainingService.updateTraining(this.selectedTrainingId, updateData).subscribe({
        next: (response) => {
          console.log('✅ Update successful:', response);
          this.toastr.success('Training updated successfully');
          this.closeModal();
          this.loadTrainings();
        },
        error: (error) => {
          console.error('❌ Update error:', error);
          this.toastr.error(error.error?.message || 'Failed to update training');
        }
      });
    } else {
      // Add new training
      this.trainingService.createTraining(requestData).subscribe({
        next: (response) => {
          console.log('✅ Create successful:', response);
          this.toastr.success('Training added successfully');
          this.closeModal();
          this.loadTrainings();
        },
        error: (error) => {
          console.error('❌ Create error:', error);
          this.toastr.error(error.error?.message || 'Failed to create training');
        }
      });
    }
  }

  deleteTraining(id: number): void {
    if (confirm('Are you sure you want to delete this training?')) {
      this.trainingService.deleteTraining(id).subscribe({
        next: () => {
          this.toastr.success('Training deleted successfully');
          this.loadTrainings();
        },
        error: (error) => {
          console.error('Error deleting training:', error);
          this.toastr.error('Failed to delete training');
        }
      });
    }
  }

  trackById(index: number, item: any): number {
    return item.id;
  }
}