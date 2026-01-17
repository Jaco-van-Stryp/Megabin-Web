import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService, GetUser } from '../../../services';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { UserTable } from "../user-table/user-table";
@Component({
  selector: 'app-admin-dashboard',
  imports: [TableModule, TagModule, UserTable],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
})
export class AdminDashboard {}
