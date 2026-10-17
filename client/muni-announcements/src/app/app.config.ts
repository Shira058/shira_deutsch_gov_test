import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';

// Angular 22 defaults cover the rest: zoneless change detection, and HttpClient (fetch backend) provided in root.
export const appConfig: ApplicationConfig = {
  providers: [provideBrowserGlobalErrorListeners()],
};
