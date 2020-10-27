import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
	selector: 'app-verify-email',
	templateUrl: './verify-email.component.html'
})
export class VerifyEmailComponent implements OnInit {
	user: User;
	constructor(
		private route: ActivatedRoute,
		private authService: AuthService
	) { }

	ngOnInit() {
		this.route.data.subscribe(res => {
			this.user = res.user
		});

		console.log(this.user);
		console.log("test");
	}

	getEmailVerifyToken(){
		console.log("send email to user now.");
	}
}