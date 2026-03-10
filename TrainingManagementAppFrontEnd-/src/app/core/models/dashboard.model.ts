import { UserTraining } from './user-training.model';

export interface Dashboard {
  welcomeMessage: string;
  pendingCount: number;
  inProgressCount: number;
  delayedCount: number;
  completedCount: number;
  myTrainings: UserTraining[];
}