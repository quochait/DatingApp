import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AuthService } from './auth.service';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl + 'user';

  constructor(private http: HttpClient, private authService: AuthService) {}

  getUsers(page?: number, itemsPerPage?: number): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }

    return this.http
      .get<User[]>(this.baseUrl, { observe: 'response', params })
      .pipe(
        map(response => {
          paginatedResult.result = response.body;

          if (response.headers.get('Pagination') != null) {
            const header = JSON.parse(response.headers.get('Pagination'));
            const tmp: Pagination = {};

            tmp.currentPage = header.CurrentPage;
            tmp.itemsPerPage = header.ItemsPerPage;
            tmp.totalPages = header.TotalPages;
            tmp.totalItems = header.TotalItems;

            paginatedResult.pagination = tmp;
          }

          return paginatedResult;
        })
      );
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + '/' + id);
  }

  updateUser(user: User) {
    return this.http.put(this.baseUrl + '/' + this.authService.decodeToken.nameid, user);
  }

  setMainPhoto(id: number) {
    return this.http.post(this.baseUrl + '/' + this.authService.decodeToken.nameid + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(id: number) {
    return this.http.delete(this.baseUrl + '/' + this.authService.decodeToken.nameid + '/photos/' + id);
  }
}
