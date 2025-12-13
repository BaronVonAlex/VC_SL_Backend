using System.Security.Cryptography;
using System.Text;

namespace VC_SL.Services;

public interface IHmacService
{
    string GenerateSignature(string payload);
    bool VerifySignature(string payload, string signature);
}

public class HmacService(IConfiguration config) : IHmacService
{
    private readonly string _secretKey = config["HmacSecret"] ?? throw new InvalidOperationException("HmacSecret not configured");

    public string GenerateSignature(string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var signature = Convert.ToBase64String(hash);

        return signature;
    }

    public bool VerifySignature(string payload, string signature)
    {
        try
        {
            var expectedSignature = GenerateSignature(payload);

            var match = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature),
                Encoding.UTF8.GetBytes(signature)
            );

            return match;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

public class HmacValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IHmacService hmacService)
    {
        if (context.Request.Method == "OPTIONS")
        {
            await next(context);
            return;
        }

        if (context.Request.Method == "GET")
        {
            await next(context);
            return;
        }


        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        var signature = context.Request.Headers["X-HMAC-Signature"].FirstOrDefault();

        if (string.IsNullOrEmpty(signature) || !hmacService.VerifySignature(body, signature))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid HMAC signature" });
            return;
        }

        await next(context);
    }
}