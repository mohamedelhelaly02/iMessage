import { Component, inject, OnDestroy } from '@angular/core';
import { ConversationStore } from '../../services/store';

@Component({
  imports: [],
  selector: 'app-conversation',
  styleUrl: './conversation.css',
  templateUrl: './conversation.html',
})
export class Conversation implements OnDestroy {
  private readonly _conversationStore = inject(ConversationStore);

  ngOnDestroy(): void {
    this._conversationStore.resetState();  
  }
}
