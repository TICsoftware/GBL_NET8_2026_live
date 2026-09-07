using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;

public class CspMiddleware
{
    private readonly RequestDelegate _next;

    public CspMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var nonceBytes = RandomNumberGenerator.GetBytes(32);
        var nonce = Convert.ToBase64String(nonceBytes);

        context.Items["CSPNonce"] = nonce;

        var csp =
            "default-src 'self'; " +

            "script-src 'self' " +
            "https://www.google.com " +
            "https://www.gstatic.com " +
            "https://www.googletagmanager.com " +
            "https://cdn.jsdelivr.net " +
            "https://analytics.google.com " +
            "https://www.google.co.in/ads/ga-audiences " +
            "https://code.jquery.com " +
            $"'nonce-{nonce}'; " +

            "style-src 'self' 'unsafe-inline' " +
            "https://fonts.googleapis.com " +
            "https://cdn.jsdelivr.net; " +

            "font-src 'self' " +
            "https://fonts.gstatic.com " +
            "data:; " +

            "img-src 'self' " +
            "https://i.ytimg.com " +
            "https://www.googletagmanager.com " +
            "data:; " +

            "media-src 'self'; " +

            "connect-src 'self' " +
            "https://www.google.com " +
            "https://www.google-analytics.com " +
            "https://analytics.google.com " +
            "https://www.google.co.in/ads/ga-audiences " +
            "https://cdn.jsdelivr.net; " +

            "frame-ancestors 'self' " +

            "frame-src 'self' " +
            "https://www.google.com " +
            "https://www.youtube.com " +

            "object-src 'none'; " +
            "base-uri 'self'; " +
            "form-action 'self';";

        context.Response.Headers["Content-Security-Policy"] = csp;

        await _next(context);
    }
}