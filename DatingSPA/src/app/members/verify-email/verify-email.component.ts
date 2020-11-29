import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { error } from 'protractor';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
	selector: 'app-verify-email',
	templateUrl: './verify-email.component.html'
})
export class VerifyEmailComponent implements OnInit {
	user: User;
	token: string;

	constructor(
		private route: ActivatedRoute,
		private authService: AuthService,
		private userService: UserService,
		private alertify: AlertifyService,
		private router: Router
	) { }

	ngOnInit() {
		this.route.data.subscribe(res => {
			this.user = res.user;
			this.token = res.token;
		});

		if(this.token){
			this.userService.checkTokenEmail(this.token).subscribe(
				res => {
					this.alertify.success("Verify successful");
					this.router.navigate(["/member/edit"]);
				},
				error => {
					this.alertify.error(error);
					this.router.navigate(["/member/edit"]);
				}
			)
		}
	}

	getEmailVerifyToken(){
		this.userService.getEmaiVerifyToken().subscribe(res => {
			this.alertify.success("Please check email: " + this.user.email);
		}, error => {
			this.alertify.error(JSON.stringify(error));
		})
	}
}