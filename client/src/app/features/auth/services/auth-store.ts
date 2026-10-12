import { computed, DestroyRef, inject, Service, signal } from '@angular/core';
import { AuthResponse, LoginCredentails, RegisterRequest, User } from '../auth.models';
import { AuthApi } from './auth-api';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, finalize, Observable, of, tap } from 'rxjs';

@Service()
export class AuthStore {
  private readonly _auth = inject(AuthApi);
  private readonly _router = inject(Router);
  private readonly _destroyRef = inject(DestroyRef);

  private readonly _user = signal<User | null>(null);
  private readonly _accessToken = signal<string | null>(null);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);

  readonly user = this._user.asReadonly();
  readonly accessToken = this._accessToken.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly error = this._error.asReadonly();

  readonly isAuthenticated = computed(() => this._user() !== null && this._accessToken() !== null);

  login(credentials: LoginCredentails): Observable<AuthResponse | null> {
    this._isLoading.set(true);
    this._error.set(null);

    return this._auth.login(credentials).pipe(
      takeUntilDestroyed(this._destroyRef),
      tap((response: AuthResponse) => {
        this._accessToken.set(response.token);
        this._user.set({
          email: response.email,
          userName: response.userName,
          displayName: response.displayName,
        });
        this._router.navigate(['/conversations']);
      }),
      catchError((e) => {
        const message = e?.error?.message || 'Login failed. Please check your credentials.';
        this._error.set(message);
        return of(null);
      }),
      finalize(() => this._isLoading.set(false)),
    );
  }

  register(request: RegisterRequest): Observable<void | null> {
    this._isLoading.set(true);
    this._error.set(null);

    return this._auth.register(request).pipe(
      takeUntilDestroyed(this._destroyRef),
      tap(() => {
        this._router.navigate(['/auth/login']);
      }),
      catchError((e) => {
        const message = e?.error?.message || 'Login failed. Please check your credentials.';
        this._error.set(message);
        return of(null);
      }),
      finalize(() => this._isLoading.set(false)),
    );
  }

  restoreSession(): Observable<AuthResponse | null> {
    this._isLoading.set(true);
    this._error.set(null);

    return this._auth.refresh().pipe(
      takeUntilDestroyed(this._destroyRef),
      tap((res: AuthResponse) => {
        this._accessToken.set(res.token);
        this._user.set({
          email: res.email,
          userName: res.userName,
          displayName: res.displayName,
        });
      }),
      catchError((e) => {
        const message = e?.error?.message || 'Session expired. Please log in again.';
        this._error.set(message);
        return of(null);
      }),
      finalize(() => this._isLoading.set(false)),
    );
  }
}
