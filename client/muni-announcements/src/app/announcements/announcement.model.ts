/** Mirrors the JSON contract of `GET /api/announcements/{authorityCode}`. */
export interface AnnouncementsResponse {
  authorityCode: string;
  authorityName: string;
  announcements: Announcement[];
}

export interface Announcement {
  id: string;
  title: string;
  /** Plain text produced by the server; still rendered via interpolation only (defense in depth). */
  summary: string;
  categoryName: string;
  isUrgent: boolean;
  /** Already formatted by the server as dd/MM/yyyy in Israel time, so the client does no date math. */
  publishDate: string;
}
