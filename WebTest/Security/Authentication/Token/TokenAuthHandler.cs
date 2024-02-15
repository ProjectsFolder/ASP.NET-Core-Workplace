﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace WebTest.Security.Authentication.Token
{
    public class TokenAuthHandler(IOptionsMonitor<TokenAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : AuthenticationHandler<TokenAuthOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var appToken = Options.Token;
            if (appToken == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid token."));
            }

            if (!Request.Headers.TryGetValue(Options.HeaderName, out StringValues token))
            {
                return Task.FromResult(AuthenticateResult.Fail($"Missing header: {Options.HeaderName}"));
            }

            if (token == appToken)
            {
                var claims = new List<Claim>()
                {
                    new("Username", "System")
                };
                var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
            }

            return Task.FromResult(AuthenticateResult.Fail("Invalid token."));
        }
    }
}
