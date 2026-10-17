import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Announcement, AnnouncementsResponse } from './announcement.model';
import { ResidentAnnouncements } from './resident-announcements';

describe('ResidentAnnouncements', () => {
  let fixture: ComponentFixture<ResidentAnnouncements>;
  let httpTesting: HttpTestingController;

  /** Renders the component for an authority and answers its single API request with `announcements`. */
  async function renderWith(
    authorityCode: string,
    announcements: Announcement[],
  ): Promise<HTMLElement> {
    fixture.componentRef.setInput('authorityCode', authorityCode);
    // Synchronous tick: whenStable() would wait forever, since the pending request keeps the app unstable.
    TestBed.tick();

    const response: AnnouncementsResponse = {
      authorityCode,
      authorityName: 'רשות לבדיקה',
      announcements,
    };
    httpTesting.expectOne(`/api/announcements/${authorityCode}`).flush(response);
    await fixture.whenStable();

    return fixture.nativeElement as HTMLElement;
  }

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClientTesting()] });
    fixture = TestBed.createComponent(ResidentAnnouncements);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('shows "אין הודעות חדשות" when the authority has no active announcements (US-5)', async () => {
    const element = await renderWith('7000', []);

    expect(element.querySelector('.announcements__empty')?.textContent?.trim()).toBe(
      'אין הודעות חדשות',
    );
    expect(element.querySelector('ul')).toBeNull();
  });

  it('renders HTML in announcement fields as text, never as DOM (US-4, XSS)', async () => {
    const payload = '<img src=x onerror="alert(1)">';
    const element = await renderWith('6500', [
      {
        id: 'a-1013',
        title: payload,
        summary: payload,
        categoryName: 'אירועים',
        isUrgent: false,
        publishDate: '16/10/2026',
      },
    ]);

    expect(element.querySelector('img')).toBeNull();
    expect(element.querySelector('h3')?.textContent?.trim()).toBe(payload);
    expect(element.querySelector('mat-card-content p')?.textContent?.trim()).toBe(payload);
  });
});
