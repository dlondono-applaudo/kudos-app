import { HttpInterceptorFn } from '@angular/common/http';
import { timeout } from 'rxjs';

const TIMEOUT_MS = 30_000;

export const timeoutInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(timeout(TIMEOUT_MS));
};
