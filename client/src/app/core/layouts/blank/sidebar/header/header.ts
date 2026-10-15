import { Component, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  imports: [FormsModule, TranslatePipe],
  selector: 'app-header',
  styleUrl: './header.css',
  templateUrl: './header.html',
})
export class Header {
  searchQuery = signal('');
  searchChange = output<string>();

  onSearchInput(e: InputEvent) {
    const searchInput = e.target as HTMLInputElement;
    const searchQuery = searchInput.value;
    this.searchQuery.set(searchQuery);
    this.searchChange.emit(searchQuery);
  }
}
