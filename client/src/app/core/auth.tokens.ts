import { HttpContextToken } from '@angular/common/http';

export const SKIP_CREDENTIALS = new HttpContextToken<boolean>(() => false);
