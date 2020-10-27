import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { Pagination } from 'src/app/_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  pagination: Pagination = {};

  constructor(private alertify: AlertifyService, private userService: UserService, private route: ActivatedRoute) {}

  ngOnInit() {
    this.loadUsers();
    console.log(this.loadUsers());
  }

  pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    this.loadUsers(this.pagination.currentPage, this.pagination.itemsPerPage);
  }

  loadUsers(page: number = 1, itemsPerPage: number = 5) {
    this.userService.getUsers(page, itemsPerPage).subscribe(
      response => {
        this.users = response.result;
        this.pagination = response.pagination;
      },
      error => {
        this.alertify.error(error);
      }
    );
  }
}
