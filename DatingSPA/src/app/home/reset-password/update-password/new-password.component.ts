import { Route } from '@angular/compiler/src/core';
import { Component, Injectable, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-new-password',
  templateUrl: './new-password.componet.html'
})

export class NewPassword implements OnInit {
  newPasswordForm: FormGroup;
  token: string;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private alertifyService: AlertifyService
  ){ }

  ngOnInit(){
    console.log("test");
    this.createNewPasswordForm();
    this.token = this.route.snapshot.paramMap.get('token');
    console.log(this.token);
  }

  createNewPasswordForm() {
    this.newPasswordForm = this.formBuilder.group(
      {
        password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(12)]],
        confirmPassword: ['', Validators.required]
      },
      { validator: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : { mismatch: true };
  }

  cancel(){}

  updatePassword(){
    let model = Object.assign({}, this.newPasswordForm.value);
    let newPasswordToUpdate = model["password"];
    let email = localStorage.getItem("email");

    if(!email){
      this.alertifyService.error("Send token reset password to email first!!!");
      this.router.navigate(['/']);
    }
    else{
      this.authService.newPassword(newPasswordToUpdate, email, this.token).subscribe(() => {
        this.alertifyService.success("Reset password success.");
      }, error => {
        this.alertifyService.error(error);
      });
      console.log(model["password"]);
    }
  }
}