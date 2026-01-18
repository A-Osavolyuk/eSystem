import {Address} from './address.interface';

export interface User {
  username: string
  email: string,
  phone: string | null,
  address: Address | null
}
