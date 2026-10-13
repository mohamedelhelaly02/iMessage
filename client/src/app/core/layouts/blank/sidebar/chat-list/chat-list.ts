import { Component, inject, OnInit } from '@angular/core';
import { ConversationStore } from '../../../../../features/chat/services/store';
import { IConversation } from '../../../../../features/chat/models/conversation';
import { AuthStore } from '../../../../../features/auth/services/store';

@Component({
  imports: [],
  selector: 'app-chat-list',
  styleUrl: './chat-list.css',
  templateUrl: './chat-list.html',
})
export class ChatList implements OnInit {
  selectConversation(conv: IConversation) {
    throw new Error('Method not implemented.');
  }
  private readonly _conversationStore = inject(ConversationStore);
  private readonly _authStore = inject(AuthStore);
  readonly conversations = this._conversationStore.conversations;
  readonly currentUser = this._authStore.user;

  ngOnInit(): void {
    this._conversationStore.loadConversations().subscribe();
  }

  getOtherParticipant(conversation: IConversation) {
    return conversation.participants.find((p) => p.userId !== this.currentUser()?.id);
  }

  isGroup(conversation: IConversation) {
    return conversation.conversationType === 'Group';
  }

  isUserOnline(conversation: IConversation) {
    var otherParticipant = this.getOtherParticipant(conversation);
    if (!otherParticipant) return;

    return this._authStore.isOnline(otherParticipant.userId);
  }

  getDisplayName(conversation: IConversation) {
    if (conversation.conversationType === 'Group') {
      return conversation.title;
    }

    return this.getOtherParticipant(conversation)?.displayName;
  }
}
