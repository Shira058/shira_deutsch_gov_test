import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { ResidentAnnouncements } from './announcements/resident-announcements';

/** Demo host page: shows that the component is self-contained and needs only an authority code. */
@Component({
  selector: 'app-root',
  imports: [MatFormFieldModule, MatSelectModule, ResidentAnnouncements],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {
  // Test authorities seeded in the server mocks; each one demonstrates a different component state.
  protected readonly authorities = [
    { code: '6500', label: '6500 – עיריית חדרה' },
    { code: '5000', label: '5000 – עיריית תל אביב-יפו' },
    { code: '7000', label: '7000 – עיריית לוד' },
    { code: '9999', label: '9999 – רשות לא קיימת' },
  ];

  protected readonly authorityCode = signal('6500');
}
