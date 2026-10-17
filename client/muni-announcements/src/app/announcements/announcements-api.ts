import { httpResource } from '@angular/common/http';
import { Injectable, Signal } from '@angular/core';
import { AnnouncementsResponse } from './announcement.model';

@Injectable({ providedIn: 'root' })
export class AnnouncementsApi {
  /**
   * Reactive request: re-fetches automatically whenever the authority code changes.
   * Must be called in an injection context (e.g. a component field initializer).
   */
  announcementsFor(authorityCode: Signal<string>) {
    return httpResource<AnnouncementsResponse>(
      () => `/api/announcements/${encodeURIComponent(authorityCode())}`,
    );
  }
}
