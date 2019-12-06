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
    return this.httpClient.post(`${this.baseUrl}/web/Account/register`, data);
  }
  public login(email: string, password: string) {
    return this.httpClient.post<LoginResult>(`${this.baseUrl}/web/account/login`, { email, password });
  }
  public getProviders() {
    return this.httpClient.get<string[]>(`${this.baseUrl}/web/account/providers`);
  }
  public getLogins() {
    return this.httpClient.get<string[]>(`${this.baseUrl}/web/account/logins`);
  }
  public removeLogin(provider: string) {
    return this.httpClient.delete(`${this.baseUrl}/web/account/logins/${provider}`);
  }
  public currentUser() {
    return this.httpClient.get<User>(`${this.baseUrl}/web/account/current-user`);
  }
  public logout() {
    return this.httpClient.post(`${this.baseUrl}/web/account/logout`, {});
  }
}
