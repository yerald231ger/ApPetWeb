using AppWebApp.Models;
using AppWebApp.Services.Token;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AppWebApp
{
    public partial class Startup
    {
        private static readonly string secretKey = "yerald231ger@gmail.com!@Ger231";

        public void ConfigureAuth(IApplicationBuilder app)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            app.UseSimpleTokenProvider(new TokenProviderOptions
            {
                Path = "/api/token",
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                IdentityResolver = GetIdentity,
                SignInPath = "/api/register",
                Schemas = Configuration
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "ExampleIssuer",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ExampleAudience",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = "Cookie",
                CookieName = "access_token",
                TicketDataFormat = new CustomJwtDataFormat(
                   SecurityAlgorithms.HmacSha256,
                   tokenValidationParameters)
            });
        }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password, UserManager<ApplicationUser> userManager)
        {
            // Don't do this in production, obviously!

            var user = await userManager.FindByNameAsync(username);
            if (user == null)
                return await Task.FromResult<ClaimsIdentity>(null);

            var ok = await userManager.CheckPasswordAsync(user, password);
            if (!ok)
                return await Task.FromResult<ClaimsIdentity>(null);

            var claims = await userManager.GetClaimsAsync(user);
            return await Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"), claims));
        }
    }
}
