import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../../services/account/account.service';
import { LoginResult } from '../../../interfaces/login-result';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  constructor(private accountService: AccountService) {
    this.accountService.getLogins().subscribe((logins) => {
      this.userLogins = logins;
    });
    this.accountService.getProviders().subscribe((providers) => {
      this.loginProviders = providers;
    });
  }

  loginProviders: string[] = [];
  userLogins: string[] = [];

  socialLoginDone(result: LoginResult) {
    if (result.status) {
      this.accountService.getLogins().subscribe((logins) => {
        this.userLogins = logins;
      });
    } else {
    }
  }

  removeSocialLogin(provider: string) {
    this.accountService.removeLogin(provider).subscribe(() => {
      this.userLogins.splice(this.userLogins.indexOf(provider), 1);
    });
  }

  ngOnInit() {
  }

}
