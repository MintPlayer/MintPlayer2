import { User } from './account/user';

export interface LoginResult {
  status: boolean;

  platform: string;
  user: User;

  error: string;
  errorDescription: string;
}
