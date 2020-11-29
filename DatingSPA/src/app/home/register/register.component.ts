import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { User } from '../../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  user: User;
  bsConfig: BsDatepickerConfig;

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private formBuilder: FormBuilder,
    private router: Router
  ) {}

  ngOnInit() {
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.formBuilder.group(
      {
        email: ['', [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")]],
        username: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(12)]],
        password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(12)]],
        gender: ['male', Validators.required],
        dateOfBirth: [null, Validators.required],
        city: ['', Validators.required],
        knownAs: ['', Validators.required],
        country: ['', Validators.required],
        confirmPassword: ['', Validators.required]
      },
      { validator: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : { mismatch: true };
  }

  register() {
    this.user = Object.assign({}, this.registerForm.value);
    this.authService.register(this.user).subscribe(
      () => {
        this.alertify.success('Registration successful.');
      },
      error => {
        this.alertify.error(error);
      },
      () => {
        this.authService.login(this.user).subscribe(() => {
          this.router.navigate(['/members']);
        });
      }
    );
  }

  cancel() {
    this.cancelRegister.emit();
  }
}
