using System.Text.Json;
using KudosApp.Api.Services;

namespace KudosApp.Api.Middleware;

/// <summary>
/// Middleware that transparently decrypts request bodies
/// when the <c>X-Encrypted: true</c> header is present.
/// Plain requests pass through untouched.
/// </summary>
public sealed class DecryptionMiddleware
{
    private readonly RequestDelegate _next;

    public DecryptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, CryptoService crypto)
    {
        if (!IsEncryptedRequest(context.Request))
        {
            await _next(context);
            return;
        }

        // Read the encrypted envelope
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body);
        var json = await reader.ReadToEndAsync();

        var envelope = JsonSerializer.Deserialize<EncryptedEnvelope>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (envelope is null || string.IsNullOrEmpty(envelope.EncryptedKey))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid encrypted envelope." });
            return;
        }

        try
        {
            var plaintext = crypto.Decrypt(envelope.EncryptedKey, envelope.Iv, envelope.Data);

            // Replace the request body with the decrypted plaintext
            var newBody = new MemoryStream(plaintext);
            context.Request.Body = newBody;
            context.Request.ContentLength = plaintext.Length;
            context.Request.ContentType = "application/json";

            // Remove the encryption header so downstream doesn't see it
            context.Request.Headers.Remove("X-Encrypted");
        }
        catch (Exception ex)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<DecryptionMiddleware>>();
            logger.LogWarning(ex, "Failed to decrypt request body");
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { message = "Decryption failed." });
            return;
        }

        await _next(context);
    }

    private static bool IsEncryptedRequest(HttpRequest request)
        => request.Headers.TryGetValue("X-Encrypted", out var val)
           && string.Equals(val, "true", StringComparison.OrdinalIgnoreCase);

    private sealed record EncryptedEnvelope(string EncryptedKey, string Iv, string Data);
}
