import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';

/**
 * Hybrid RSA-OAEP + AES-GCM encryption service.
 * Fetches the server's RSA public key once, then encrypts every
 * outbound payload with a fresh AES-256-GCM key wrapped by RSA.
 */
@Injectable({ providedIn: 'root' })
export class EncryptionService {
  private publicKey: CryptoKey | null = null;
  private loading: Promise<CryptoKey> | null = null;

  constructor(private http: HttpClient) {}

  /** Ensure the RSA public key is loaded (cached after first call). */
  private async getPublicKey(): Promise<CryptoKey> {
    if (this.publicKey) return this.publicKey;
    if (this.loading) return this.loading;

    this.loading = this.fetchPublicKey();
    this.publicKey = await this.loading;
    this.loading = null;
    return this.publicKey;
  }

  private async fetchPublicKey(): Promise<CryptoKey> {
    const res = await firstValueFrom(
      this.http.get<{ publicKey: string }>(`${environment.apiUrl}/crypto/public-key`),
    );

    const pem = res.publicKey;
    const binaryDer = this.pemToArrayBuffer(pem);

    return crypto.subtle.importKey(
      'spki',
      binaryDer,
      { name: 'RSA-OAEP', hash: 'SHA-256' },
      false,
      ['wrapKey'],
    );
  }

  /**
   * Encrypt a JSON-serializable payload.
   * Returns an envelope: { encryptedKey, iv, data } (all base64).
   */
  async encrypt(payload: unknown): Promise<{ encryptedKey: string; iv: string; data: string }> {
    const rsaKey = await this.getPublicKey();

    // 1. Generate a one-time AES-256-GCM key
    const aesKey = await crypto.subtle.generateKey({ name: 'AES-GCM', length: 256 }, true, [
      'encrypt',
    ]);

    // 2. Encrypt the payload
    const iv = crypto.getRandomValues(new Uint8Array(12));
    const plaintext = new TextEncoder().encode(JSON.stringify(payload));
    const cipherBuf = await crypto.subtle.encrypt({ name: 'AES-GCM', iv }, aesKey, plaintext);

    // 3. Wrap (encrypt) the AES key with the server's RSA public key
    const wrappedKey = await crypto.subtle.wrapKey('raw', aesKey, rsaKey, {
      name: 'RSA-OAEP',
    });

    return {
      encryptedKey: this.bufToBase64(wrappedKey),
      iv: this.bufToBase64(iv),
      data: this.bufToBase64(cipherBuf), // ciphertext + GCM auth tag (appended by WebCrypto)
    };
  }

  // ── helpers ────────────────────────────────────────────────
  private pemToArrayBuffer(pem: string): ArrayBuffer {
    const lines = pem
      .replace(/-----BEGIN PUBLIC KEY-----/, '')
      .replace(/-----END PUBLIC KEY-----/, '')
      .replace(/\s/g, '');
    const binary = atob(lines);
    const buf = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) buf[i] = binary.charCodeAt(i);
    return buf.buffer;
  }

  private bufToBase64(buf: ArrayBuffer): string {
    const bytes = new Uint8Array(buf);
    let binary = '';
    for (const b of bytes) binary += String.fromCharCode(b);
    return btoa(binary);
  }
}
