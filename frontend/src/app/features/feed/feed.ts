import { Component } from '@angular/core';

@Component({
  selector: 'app-feed',
  template: `
    <div class="page-header">
      <h1>Kudos Feed</h1>
      <p>See what your team is celebrating</p>
    </div>
    <p class="placeholder">Feed coming soon — Phase 2B</p>
  `,
  styles: `
    .page-header { margin-bottom: 1.5rem; }
    .page-header h1 { margin: 0; font-size: 1.5rem; color: var(--text-primary); }
    .page-header p { margin: 0.25rem 0 0; color: var(--text-secondary); }
    .placeholder { color: var(--text-secondary); font-style: italic; }
  `,
})
export class Feed {}
