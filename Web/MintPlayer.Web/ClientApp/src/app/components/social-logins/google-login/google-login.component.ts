import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { BaseLoginComponent } from '../base-login.component';

@Component({
  selector: 'app-google-login',
  templateUrl: './google-login.component.html',
  styleUrls: ['./google-login.component.scss']
})
export class GoogleLoginComponent extends BaseLoginComponent implements OnInit, OnDestroy {

  constructor(@Inject('BASE_URL') baseUrl: string) {
    super(baseUrl, "Google");
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.dispose();
  }

}
