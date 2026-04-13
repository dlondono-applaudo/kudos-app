using KudosApp.Api.Services;

namespace KudosApp.Api.Endpoints;

public static class CryptoEndpoints
{
    public static IEndpointRouteBuilder MapCryptoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/crypto/public-key", (CryptoService crypto) =>
            Results.Ok(new { publicKey = crypto.GetPublicKeyPem() }))
            .WithTags("Crypto")
            .AllowAnonymous();

        return app;
    }
}
