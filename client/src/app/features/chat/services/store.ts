import { Service, signal } from '@angular/core';

@Service()
export class ConversationStore {
  private readonly _typingConversations = signal<Set<string>>(new Set());

  readonly typingConversations = this._typingConversations.asReadonly();

  private updateConversationTyping(conversationId: string): void {
    this._typingConversations.update((conversations) => {
      const updatedTypingConversations = new Set(conversations);
      updatedTypingConversations.add(conversationId);
      return updatedTypingConversations;
    });
  }

  private deleteConversationTyping(conversationId: string): void {
    this._typingConversations.update((typingConversations) => {
      const updatedTypingConversations = new Set(typingConversations);
      updatedTypingConversations.delete(conversationId);
      return updatedTypingConversations;
    });
  }

  conversationStartedTyping(conversationId: string): void {
    if (!conversationId) return;
    this.updateConversationTyping(conversationId);
  }

  conversationStoppedTyping(conversationId: string): void {
    if (!conversationId) return;
    this.deleteConversationTyping(conversationId);
  }
}
