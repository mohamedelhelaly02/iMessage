import { HttpInterceptorFn } from '@angular/common/http';

export const cookiesInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.method === 'OPTIONS')
    return next(req);

  const clonedRequest = req.clone({ withCredentials: true });

  return next(clonedRequest);
};
