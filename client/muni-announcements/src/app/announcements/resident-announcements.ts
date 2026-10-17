import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AnnouncementsApi } from './announcements-api';

// Keeps the heading id unique if the host page embeds the component more than once.
let nextId = 0;

@Component({
  selector: 'app-resident-announcements',
  imports: [MatButtonModule, MatCardModule, MatIconModule, MatProgressBarModule],
  templateUrl: './resident-announcements.html',
  styleUrl: './resident-announcements.scss',
  // OnPush is the default in Angular 22; stated explicitly to document that the view is signal-driven.
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ResidentAnnouncements {
  readonly authorityCode = input.required<string>();

  protected readonly headingId = `resident-announcements-title-${nextId++}`;
  protected readonly resource = inject(AnnouncementsApi).announcementsFor(this.authorityCode);

  protected readonly announcements = computed(() =>
    this.resource.hasValue() ? this.resource.value().announcements : [],
  );

  /** Text for the persistent live region. Errors are left to role="alert" so they are not announced twice. */
  protected readonly statusMessage = computed(() => {
    if (this.resource.isLoading()) return 'טוען הודעות…';
    if (!this.resource.hasValue()) return '';

    const count = this.announcements().length;
    if (count === 0) return 'אין הודעות חדשות';
    return count === 1 ? 'נמצאה הודעה אחת' : `נמצאו ${count} הודעות`;
  });
}
