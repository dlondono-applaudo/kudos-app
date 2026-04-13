using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace KudosApp.Api.Services;

/// <summary>
/// Manages a per-instance RSA key pair and decrypts hybrid-encrypted payloads
/// sent by the frontend (AES-GCM data + RSA-OAEP wrapped key).
/// </summary>
public sealed class CryptoService : IDisposable
{
    private readonly RSA _rsa = RSA.Create(2048);

    /// <summary>RSA public key exported as PEM (SPKI).</summary>
    public string GetPublicKeyPem()
        => _rsa.ExportSubjectPublicKeyInfoPem();

    /// <summary>
    /// Decrypts an encrypted envelope produced by the frontend.
    /// Envelope shape: { encryptedKey: base64, iv: base64, data: base64 }
    /// </summary>
    public byte[] Decrypt(string encryptedKeyB64, string ivB64, string dataB64)
    {
        // 1. Unwrap the AES key with our RSA private key
        var wrappedKey = Convert.FromBase64String(encryptedKeyB64);
        var aesKey = _rsa.Decrypt(wrappedKey, RSAEncryptionPadding.OaepSHA256);

        // 2. Decode IV and ciphertext
        var iv = Convert.FromBase64String(ivB64);
        var ciphertext = Convert.FromBase64String(dataB64);

        // AES-GCM: last 16 bytes are the auth tag
        const int tagSize = 16;
        var tag = ciphertext.AsSpan(ciphertext.Length - tagSize);
        var encrypted = ciphertext.AsSpan(0, ciphertext.Length - tagSize);

        var plaintext = new byte[encrypted.Length];
        using var aesGcm = new AesGcm(aesKey, tagSize);
        aesGcm.Decrypt(iv, encrypted, tag, plaintext);

        return plaintext;
    }

    public void Dispose() => _rsa.Dispose();
}
