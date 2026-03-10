export interface Training {
  id: number;
  trainingName: string;
  trainingUrl: string;
  mode: TrainingMode;
  platform: string;
  expectedStartDate: Date;
  expectedEndDate: Date;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export enum TrainingMode {
  InstructorLed = 1,
  Virtual = 2
}

export interface CreateTrainingRequest {
  trainingName: string;
  trainingUrl: string;
  mode: TrainingMode;
  platform: string;
  expectedStartDate: Date;
  expectedEndDate: Date;
}

export interface UpdateTrainingRequest {
  id: number;
  trainingName: string;
  trainingUrl: string;
  mode: TrainingMode;
  platform: string;
  expectedStartDate: Date;
  expectedEndDate: Date;
  isActive: boolean;
}