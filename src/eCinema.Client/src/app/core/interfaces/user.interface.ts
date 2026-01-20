import {Address} from './address.interface';

export interface User {
  username: string
  email: string,
  phone?: string,
  address?: Address | null
}
