import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: './app-reset-password',
  templateUrl: './reset-password.component.html'
})
export class ResetPasswordComponent implements OnInit {
  registerMode = false;
  resetMode = false;
  values: any;
  resetForm: FormGroup;
  @Output() cancelResetPassword = new EventEmitter();

  constructor(
    private authService: AuthService,
    private formBuilder: FormBuilder,
    private alertifyService: AlertifyService
  ) {}

  ngOnInit() {
    this.initNgForm();
  }

  initNgForm(){
    this.resetForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")]]
    });
  }

  cancel(){
    this.cancelResetPassword.emit();
  }

  getTokenReset(){
    let email = this.resetForm.value.email;
    this.authService.getTokenResetPassword(email).subscribe(() => {
      this.alertifyService.success("Please check email!");
    }, error => {
      this.alertifyService.error(error);
    });
    // console.log(this.resetForm.value.email);
  }
}
