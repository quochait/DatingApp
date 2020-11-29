import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  summaryMode = true;
  resetMode = false;
  values: any;
  constructor() {}

  ngOnInit() {}

  registerToggle() {
    this.registerMode = !this.registerMode;
    this.summaryMode = !this.summaryMode;
  }

  cancelRegisterMode() {
    this.registerToggle();
  }

  resetToggle(){
    this.resetMode = !this.resetMode;
    this.summaryMode = !this.summaryMode;
  }

  cancelResetPasswordMode(){
    this.resetToggle();
  }
}
