import { Component, OnInit, Input, OnDestroy, EventEmitter, Inject, Output } from '@angular/core';
import { LoginResult } from '../../../interfaces/login-result';
import { BaseLoginComponent } from '../base-login.component';

@Component({
  selector: 'app-facebook-login',
  templateUrl: './facebook-login.component.html',
  styleUrls: ['./facebook-login.component.scss']
})
export class FacebookLoginComponent extends BaseLoginComponent implements OnInit, OnDestroy {
  constructor(@Inject('BASE_URL') baseUrl: string) {
    super(baseUrl, 'Facebook');
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.dispose();
  }
}
