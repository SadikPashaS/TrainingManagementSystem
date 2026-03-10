import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface DashboardData {
  welcomeMessage: string;
  pendingCount: number;
  inProgressCount: number;
  delayedCount: number;
  completedCount: number;
  myTrainings: any[];
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getDashboard(): Observable<DashboardData> {
    return this.http.get<DashboardData>(`${this.apiUrl}/Dashboard`);
  }

  // Helper method to get all trainings (for mapping)
  getAllTrainings(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Trainings`);
  }
}