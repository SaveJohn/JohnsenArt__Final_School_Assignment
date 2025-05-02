using System.Security.Cryptography;
using System.Text;

namespace IntegrationTests.Features.StripeTests.IntegrationTests.Helpers;

public class StripeTestHelpers
{
    public static string BuildTestHeader(string payload, string secret, long? timestamp = null)
    {
        var ts = timestamp ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var signed = $"{ts}.{payload}";
        var key = Encoding.UTF8.GetBytes(secret);
        var data = Encoding.UTF8.GetBytes(signed);

        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(data);
        var v1 = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        // Stripe’s header format: t=<ts>,v1=<hex-hmac>
        return $"t={ts},v1={v1}";
    }
}