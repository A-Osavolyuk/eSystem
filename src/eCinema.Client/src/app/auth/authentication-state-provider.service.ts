import {Injectable, signal} from '@angular/core';
import {AuthenticationState} from '../core/interfaces/authentication-state.interface';
import {UserInfo} from '../core/interfaces/userinfo.interface';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationStateProvider {
  private authenticationState = signal<AuthenticationState>({
    isAuthenticated: false,
    user: null
  });

  public readonly state = this.authenticationState.asReadonly();

  public signOut = () => {
    this.authenticationState.set({
      isAuthenticated: false,
      user: null
    });
  }

  public signIn = (userInfo: UserInfo) => {
    this.authenticationState.set({
      isAuthenticated: true,
      user: {
        id: userInfo.sub,
        username: userInfo.preferred_username ?? '',
        email: userInfo.email ?? '',
        phone: userInfo.phone,
        address: userInfo.address ? {
          streetAddress: userInfo.address.street_address,
          region: userInfo.address.region,
          countryCode: userInfo.address.country_code,
          locality: userInfo.address.locality,
          postalCode: userInfo.address.postal_code,
        } : null
      }
    });
  }
}
