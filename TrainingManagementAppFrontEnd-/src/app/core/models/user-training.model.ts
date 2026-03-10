export interface UserTraining {
  id: number;
  userId: number;
  userName: string;
  trainingId: number;
  trainingName: string;
  status: TrainingStatus;
  expectedCompletionDate?: Date;
  comments?: string;
  assignedAt: Date;
  lastUpdatedAt?: Date;
}

export enum TrainingStatus {
  Pending = 1,
  InProgress = 2,
  Delayed = 3,
  Completed = 4
}

export interface UpdateStatusRequest {
  userTrainingId: number;
  status: TrainingStatus;
  expectedCompletionDate?: string;
  comments?: string;
}

export interface AssignTrainingRequest {
  userId: number;
  trainingId: number;
}