import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './features/auth/login/login';
import { UserDashboardComponent } from './features/dashboard/user-dashboard/user-dashboard';
import { UpdateStatusComponent } from './features/trainings/update-status/update-status';
import { TrainingManagementComponent } from './features/admin/training-management/training-management';
import { UserOverviewComponent } from './features/admin/user-overview/user-overview';
import { UserAssignmentsComponent } from './features/admin/user-assignments/user-assignments';

import { AuthGuard } from './core/guards/auth.guard';
import { AdminGuard } from './core/guards/admin.guard';

const routes: Routes = [
  // Public routes
  { path: 'login', component: LoginComponent },
  
  // User routes
  { 
    path: 'dashboard', 
    component: UserDashboardComponent, 
    canActivate: [AuthGuard] 
  },
  { 
    path: 'update-status/:id', 
    component: UpdateStatusComponent, 
    canActivate: [AuthGuard] 
  },
  
  // Admin routes
  { 
    path: 'admin', 
    redirectTo: '/admin/trainings', 
    pathMatch: 'full' 
  },
  { 
    path: 'admin/trainings', 
    component: TrainingManagementComponent, 
    canActivate: [AuthGuard, AdminGuard] 
  },
  { 
    path: 'admin/users', 
    component: UserOverviewComponent, 
    canActivate: [AuthGuard, AdminGuard] 
  },
  { 
    path: 'admin/assignments', 
    component: UserAssignmentsComponent, 
    canActivate: [AuthGuard, AdminGuard] 
  },
  
  // Default routes
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }