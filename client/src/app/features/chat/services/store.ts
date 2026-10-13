import { DestroyRef, inject, Service, signal } from '@angular/core';
import { ConversationApi } from './conversation-api';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, finalize, Observable, of, tap } from 'rxjs';
import { IConversation } from '../models/conversation';

@Service()
export class ConversationStore {
  private readonly _api = inject(ConversationApi);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);
  private readonly _typingConversations = signal<Set<string>>(new Set());
  private readonly _destroyRef = inject(DestroyRef);
  private readonly _conversations = signal<IConversation[]>([]);

  readonly isLoading = this._isLoading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly typingConversations = this._typingConversations.asReadonly();
  readonly conversations = this._conversations.asReadonly();

  loadConversations(): Observable<IConversation[]> {
    this._isLoading.set(true);
    this._error.set(null);
    return this._api.getConversations().pipe(
      takeUntilDestroyed(this._destroyRef),
      tap((data: IConversation[]) => {
        this._conversations.set(data);
        console.log('loaded conversations');
      }),
      catchError((err) => {
        const message = err?.error?.message || 'Unable to load conversations.';
        this._error.set(message);
        return of([]);
      }),
      finalize(() => this._isLoading.set(false)),
    );
  }

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
