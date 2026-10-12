import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BlankNavbar } from '../../../shared/components/blank-navbar/blank-navbar';

@Component({
  imports: [RouterOutlet, BlankNavbar],
  selector: 'app-blank',
  styleUrl: './blank.css',
  templateUrl: './blank.html',
})
export class Blank {
  isRtl() {
    return "";
  }
  onLogout() {
  }
}
