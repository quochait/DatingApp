import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({providedIn: 'root'})
export class TokenInterceptor implements HttpInterceptor{
	constructor(){}

	intercept(request, next){
		const token = localStorage.getItem('token');
		request = request.clone({
			setHeaders: {
				Authorization: `Bearer ${token}`
			}
		})
		return next.handle(request);
	}
}