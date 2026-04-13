import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { from, switchMap } from 'rxjs';
import { EncryptionService } from '../services/encryption.service';

/**
 * HTTP interceptor that encrypts POST / PUT / PATCH request bodies
 * using hybrid RSA+AES-GCM encryption before they leave the browser.
 *
 * Requests to the crypto/public-key endpoint are excluded to avoid
 * circular dependency (we need the key to encrypt).
 */
export const encryptionInterceptor: HttpInterceptorFn = (req, next) => {
  const encryption = inject(EncryptionService);

  const hasBody = req.body && ['POST', 'PUT', 'PATCH'].includes(req.method);
  const isCryptoEndpoint = req.url.includes('/crypto/public-key');

  if (!hasBody || isCryptoEndpoint) {
    return next(req);
  }

  return from(encryption.encrypt(req.body)).pipe(
    switchMap(envelope => {
      const encrypted = req.clone({
        body: envelope,
        setHeaders: {
          'X-Encrypted': 'true',
          'Content-Type': 'application/json',
        },
      });
      return next(encrypted);
    }),
  );
};
