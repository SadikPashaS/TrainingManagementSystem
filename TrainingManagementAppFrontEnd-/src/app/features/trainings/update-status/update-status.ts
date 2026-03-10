import { Component, OnInit, ChangeDetectorRef } from '@angular/core'; // Add ChangeDetectorRef
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserTrainingService } from '../../../core/services/user-training.service';
import { ToastrService } from 'ngx-toastr';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-update-status',
  templateUrl: './update-status.html',
  styleUrls: ['./update-status.css'],
  standalone: false
})
export class UpdateStatusComponent implements OnInit {
  statusForm: FormGroup;
  userTrainingId!: number;
  trainingName = '';
  loading = false;
  showExpectedDate = false;

  statusOptions = [
    { value: 1, label: 'Pending' },
    { value: 2, label: 'In Progress' },
    { value: 3, label: 'Delayed' },
    { value: 4, label: 'Completed' }
  ];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private userTrainingService: UserTrainingService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef // Add this
  ) {
    console.log('🔷 UpdateStatusComponent constructor called');
    
    this.statusForm = this.fb.group({
      userTrainingId: [''],
      status: ['', Validators.required],
      expectedCompletionDate: [''],
      comments: ['']
    });
  }

  ngOnInit(): void {
    console.log('🔷 ngOnInit called');
    
    this.userTrainingId = +this.route.snapshot.params['id'];
    console.log('🔷 Training ID from URL:', this.userTrainingId);
    
    this.statusForm.patchValue({ userTrainingId: this.userTrainingId });
    this.loadTrainingDetails();

    this.statusForm.get('status')?.valueChanges.subscribe(value => {
      console.log('🔷 Status changed to:', value);
      this.showExpectedDate = value === 3;
      if (this.showExpectedDate) {
        this.statusForm.get('expectedCompletionDate')?.setValidators([Validators.required]);
      } else {
        this.statusForm.get('expectedCompletionDate')?.clearValidators();
        this.statusForm.get('expectedCompletionDate')?.setValue('');
      }
      this.statusForm.get('expectedCompletionDate')?.updateValueAndValidity();
      this.cdr.detectChanges(); // Force change detection
    });
  }

  loadTrainingDetails(): void {
    console.log('🔷 Loading training details...');
    
    this.userTrainingService.getMyTrainings().subscribe({
      next: (trainings) => {
        console.log('✅ Trainings received:', trainings);
        const training = trainings.find(t => t.id === this.userTrainingId);
        
        if (training) {
          console.log('✅ Found training:', training);
          this.trainingName = training.trainingName;
          
          const formData: any = {
            status: training.status,
            comments: training.comments || ''
          };
          
          if (training.expectedCompletionDate) {
            formData.expectedCompletionDate = new Date(training.expectedCompletionDate).toISOString().split('T')[0];
          }
          
          this.statusForm.patchValue(formData);
          console.log('✅ Form patched with:', formData);
          this.cdr.detectChanges(); // Force change detection
        } else {
          console.error('❌ Training not found with ID:', this.userTrainingId);
          this.toastr.error('Training not found');
        }
      },
      error: (error) => {
        console.error('❌ Error loading trainings:', error);
        this.toastr.error('Failed to load training details');
        this.cdr.detectChanges(); // Force change detection
      }
    });
  }

  onSubmit(): void {
    console.log('🔷 Submit button clicked');
    console.log('🔷 Form valid:', this.statusForm.valid);
    console.log('🔷 Form values:', this.statusForm.value);
    
    if (this.statusForm.invalid) {
      console.log('❌ Form is invalid');
      this.toastr.warning('Please fill all required fields');
      return;
    }

    this.loading = true;
    console.log('🔷 Loading set to true');
    this.cdr.detectChanges(); // Force change detection
    
    const requestData = this.statusForm.value;
    console.log('🔷 Sending request:', requestData);
    
    this.userTrainingService.updateStatus(requestData).subscribe({
      next: (response) => {
        console.log('✅ Update successful! Response:', response);
        this.toastr.success('Training status updated successfully');
        console.log('🔷 Navigating to dashboard...');
        this.router.navigate(['/dashboard']);
        this.loading = false;
        this.cdr.detectChanges(); // Force change detection
      },
      error: (error: HttpErrorResponse) => {
        console.error('❌ Update failed!');
        console.error('❌ Error status:', error.status);
        console.error('❌ Error message:', error.message);
        
        this.loading = false;
        this.cdr.detectChanges(); // Force change detection
        
        if (error.status === 0) {
          this.toastr.error('Cannot connect to server. Check if backend is running.');
        } else if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid data');
        } else if (error.status === 401) {
          this.toastr.error('Session expired. Please login again.');
          this.router.navigate(['/login']);
        } else {
          this.toastr.error(error.error?.message || 'Failed to update status');
        }
      }
    });
  }

  onCancel(): void {
    console.log('🔷 Cancel clicked');
    this.router.navigate(['/dashboard']);
  }
}