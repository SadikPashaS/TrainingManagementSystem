import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UpdateStatusRequest {
  userTrainingId: number;
  status: number;
  expectedCompletionDate?: string;
  comments?: string;
}

export interface AssignTrainingRequest {
  userId: number;
  trainingId: number;
}

@Injectable({
  providedIn: 'root'
})
export class UserTrainingService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getMyTrainings(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/UserTrainings/my-trainings`);
  }

  getAllUserTrainings(): Observable<any[]> {
    console.log('🔵 Service: Fetching all user trainings');
    return this.http.get<any[]>(`${this.apiUrl}/UserTrainings/all`);
  }

  getUserTrainings(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/UserTrainings/user/${userId}`);
  }

  updateStatus(request: UpdateStatusRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/UserTrainings/status`, request);
  }

  assignTraining(request: AssignTrainingRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/UserTrainings/assign`, request);
  }

  removeTraining(userId: number, trainingId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/UserTrainings/remove/${userId}/${trainingId}`);
  }
}