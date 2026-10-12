import { HttpInterceptorFn } from '@angular/common/http';
import { SKIP_CREDENTIALS } from '../auth.tokens';

export const cookiesInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.method === 'OPTIONS' || req.context.get(SKIP_CREDENTIALS)) return next(req);

  const clonedRequest = req.clone({ withCredentials: true });

  return next(clonedRequest);
};
