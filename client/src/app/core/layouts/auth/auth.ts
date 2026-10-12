import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthNavbar } from '../../../shared/components/auth-navbar/auth-navbar';

@Component({
  imports: [RouterOutlet, AuthNavbar],
  selector: 'app-auth',
  styleUrl: './auth.css',
  templateUrl: './auth.html',
})
export class Auth {}
