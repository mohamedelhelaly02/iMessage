import { Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageSelector } from '../language-switcher/language-switcher';

@Component({
  imports: [TranslatePipe, LanguageSelector],
  selector: 'app-auth-navbar',
  styleUrl: './auth-navbar.css',
  templateUrl: './auth-navbar.html',
})
export class AuthNavbar {}
