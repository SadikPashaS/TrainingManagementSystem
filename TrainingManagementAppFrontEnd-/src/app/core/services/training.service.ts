import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Training {
  id: number;
  trainingName: string;
  trainingUrl: string;
  mode: number;
  platform: string;
  expectedStartDate: Date;
  expectedEndDate: Date;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class TrainingService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllTrainings(): Observable<Training[]> {
    return this.http.get<Training[]>(`${this.apiUrl}/Trainings`);
  }

  getTrainingById(id: number): Observable<Training> {
    return this.http.get<Training>(`${this.apiUrl}/Trainings/${id}`);
  }

  createTraining(training: any): Observable<Training> {
    return this.http.post<Training>(`${this.apiUrl}/Trainings`, training);
  }

  updateTraining(id: number, training: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Trainings/${id}`, training);
  }

  deleteTraining(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Trainings/${id}`);
  }
}