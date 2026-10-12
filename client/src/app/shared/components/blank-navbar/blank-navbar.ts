import { Component, output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  imports: [TranslatePipe],
  selector: 'app-blank-navbar',
  styleUrl: './blank-navbar.css',
  templateUrl: './blank-navbar.html',
})
export class BlankNavbar {
  isRtl() {
    return "";
  }
  logoutEvent = output<void>();

  logout() {
    this.logoutEvent.emit();
  }
}
