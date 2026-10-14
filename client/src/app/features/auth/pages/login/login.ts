import { Component, inject } from '@angular/core';
import {
  FormsModule,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthStore } from '../../services/store';
import { LoginCredentails } from '../../models/credentails';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';

@Component({
  imports: [TranslatePipe, ReactiveFormsModule, RouterLink],
  selector: 'app-login',
  styleUrl: './login.css',
  templateUrl: './login.html',
})
export class Login {
  private readonly _fb = inject(NonNullableFormBuilder);
  private readonly _authStore = inject(AuthStore);

  readonly isLoading = this._authStore.isLoading;
  readonly errorMessage = this._authStore.error;

  readonly loginForm = this._fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const credentials = this.loginForm.getRawValue() as LoginCredentails;

    this._authStore.login(credentials).subscribe({
      error: (e: HttpErrorResponse) => {
        this._authStore.clearMessage();
      },
    });
  }
}
