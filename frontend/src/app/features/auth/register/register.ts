import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  email = '';
  password = '';
  fullName = '';
  department = '';
  error = signal('');
  loading = signal(false);

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit() {
    this.error.set('');
    this.loading.set(true);

    this.auth
      .register({
        email: this.email,
        password: this.password,
        fullName: this.fullName,
        department: this.department,
      })
      .subscribe({
        next: () => {
          this.loading.set(false);
          this.router.navigate(['/']);
        },
        error: (err) => {
          this.loading.set(false);
          const msg =
            err.error?.errors
              ? Object.values(err.error.errors).flat().join('. ')
              : err.error?.message ?? 'Registration failed. Please try again.';
          this.error.set(msg);
        },
      });
  }
}
