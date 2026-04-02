import { Component, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-bar',
  imports: [FormsModule],
  templateUrl: './search-bar.html',
  styleUrl: './search-bar.scss',
})
export class SearchBarComponent {
  readonly search = output<string>();
  searchTerm = signal('');

  onInput(value: string): void {
    this.searchTerm.set(value);
    this.search.emit(value);
  }

  onClear(): void {
    this.searchTerm.set('');
    this.search.emit('');
  }
}
