import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Guid } from 'guid-typescript';
import { AccountService } from '../../../services/account/account.service';
import { UserData } from '../../../interfaces/account/user-data';
import { User } from '../../../interfaces/account/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  constructor(private router: Router, private accountService: AccountService) {
  }

  public data: UserData = {
    user: {
      id: Guid.createEmpty()["value"],
      userName: '',
      email: '',
      pictureUrl: ''
    },
    password: '',
    passwordConfirmation: ''
  };

  public errorDescription: string = '';

  ngOnInit() {
  }

  public register() {
    this.accountService.register(this.data).subscribe(() => {
      this.accountService.login(
        this.data.user.email, this.data.password
      ).subscribe((login_result) => {
        if (login_result.status === true) {
          this.router.navigate(['/']);
          this.loginComplete.emit(login_result.user);
        } else {
          this.errorDescription = login_result.errorDescription;
        }
      });
    }, (error: HttpErrorResponse) => {
      this.errorDescription = "Something went wrong here";
    });
  }

  @Output() loginComplete: EventEmitter<User> = new EventEmitter();
}
