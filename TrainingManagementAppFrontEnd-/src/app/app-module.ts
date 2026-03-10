import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { LoadingBarModule } from '@ngx-loading-bar/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app';

// Core
import { AuthInterceptor } from './core/interceptors/auth.interceptor';

// Shared Components
import { NavbarComponent } from './shared/navbar/navbar';

// Feature Components - Auth
import { LoginComponent } from './features/auth/login/login';

// Feature Components - Dashboard
import { UserDashboardComponent } from './features/dashboard/user-dashboard/user-dashboard';

// Feature Components - Trainings
import { UpdateStatusComponent } from './features/trainings/update-status/update-status';

// Feature Components - Admin
import { TrainingManagementComponent } from './features/admin/training-management/training-management';
import { UserOverviewComponent } from './features/admin/user-overview/user-overview';
import { UserAssignmentsComponent } from './features/admin/user-assignments/user-assignments';

@NgModule({
  declarations: [
    // Core Components
    AppComponent,
    NavbarComponent,
    
    // Auth Components
    LoginComponent,
    
    // Dashboard Components
    UserDashboardComponent,
    
    // Training Components
    UpdateStatusComponent,
    
    // Admin Components
    TrainingManagementComponent,
    UserOverviewComponent,
    UserAssignmentsComponent
  ],
  imports: [
    // Angular Modules
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    
    // Toastr Module - UPDATED CONFIGURATION
    ToastrModule.forRoot({
      positionClass: 'toast-top-center',  // Center position
  timeOut: 4000,                      // 4 seconds
  extendedTimeOut: 1000,
  closeButton: true,
  progressBar: false,                  // NO progress bar
  progressAnimation: 'decreasing',
  preventDuplicates: true,
  newestOnTop: true,
  tapToDismiss: true,
  easeTime: 300,
  enableHtml: true,
  titleClass: 'toast-title',
  messageClass: 'toast-message',
  toastClass: 'custom-toast' 
                                    // Animation speed
    }),
    
    LoadingBarModule
  ],
  providers: [
    { 
      provide: HTTP_INTERCEPTORS, 
      useClass: AuthInterceptor, 
      multi: true 
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }