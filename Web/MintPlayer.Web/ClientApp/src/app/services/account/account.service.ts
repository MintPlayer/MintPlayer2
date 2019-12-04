import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserData } from '../../interfaces/account/user-data';
import { LoginResult } from '../../interfaces/login-result';
import { User } from '../../interfaces/account/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  public register(data: UserData) {
    return this.httpClient.post(`${this.baseUrl}/web/Account/register`, {
      user: {
        userName: data.user.userName,
        email: data.user.email,
        pictureUrl: data.user.pictureUrl
      },
      password: data.password,
      passwordConfirmation: data.passwordConfirmation
    });
  }
  public login(email: string, password: string) {
    return this.httpClient.post<LoginResult>(`${this.baseUrl}/web/account/login`, { email, password });
  }
  public currentUser() {
    return this.httpClient.get<User>(`${this.baseUrl}/web/account/current-user`);
  }
  public logout() {
    return this.httpClient.post(`${this.baseUrl}/api/account/logout`, {});
  }
}
