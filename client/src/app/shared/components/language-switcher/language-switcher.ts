import { Component, HostListener, inject, signal } from '@angular/core';
import { LanguageService } from '../../../core/services/language';
import { UpperCasePipe } from '@angular/common';

@Component({
  imports: [UpperCasePipe],
  selector: 'app-language-switcher',
  styleUrl: './language-switcher.css',
  templateUrl: './language-switcher.html',
})
export class LanguageSelector {
  isOpen = signal<boolean>(false);
  private readonly _languageService = inject(LanguageService);
  readonly languages = this._languageService.languages;
  readonly selectedLanguage = this._languageService.currentSelectedLang;

  toggleOpen(event: Event): void {
    event.stopPropagation();
    this.isOpen.update((v) => !v);
  }

  selectLanguage(lang: any) {
    this._languageService.setLanguage(lang);
  }

  @HostListener('document:click')
  closeOnOutsideClick(): void {
    this.isOpen.set(false);
  }
}
