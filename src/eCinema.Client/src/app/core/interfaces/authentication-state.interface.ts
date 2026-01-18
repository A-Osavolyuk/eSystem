import {User} from './user.interface';

export interface AuthenticationState {
  isAuthenticated: boolean,
  user: User | null
}
