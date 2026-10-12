import { Component, inject, output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageSelector } from '../language-switcher/language-switcher';
import { LanguageService } from '../../../core/services/language';

@Component({
  imports: [TranslatePipe, LanguageSelector],
  selector: 'app-blank-navbar',
  styleUrl: './blank-navbar.css',
  templateUrl: './blank-navbar.html',
})
export class BlankNavbar {
  readonly isRtl = inject(LanguageService).isRtl;
  logoutEvent = output<void>();

  logout() {
    this.logoutEvent.emit();
  }
}
