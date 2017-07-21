using AppWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AppWebApp.Services.Token
{

    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenProviderMiddleware(
             RequestDelegate next,
             IOptions<TokenProviderOptions> options,
             UserManager<ApplicationUser> userManager,
             ILoggerFactory loggerFactory
             )
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<TokenProviderMiddleware>();
            _userManager = userManager;
            var d = _userManager.FindByEmailAsync("yerald231ger@gmail.com").Result;
            _options = options.Value;
            ThrowIfInvalidOptions(_options);
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        public Task Invoke(HttpContext context)
        {
            // If the request path doesn't match, skip
            if (context.Request.Path.Equals(_options.Path, StringComparison.Ordinal)
                || context.Request.Path.Equals(_options.SignInPath, StringComparison.Ordinal))
            {

                // Request must be POST with Content-Type: application/x-www-form-urlencoded
                if (!context.Request.Method.Equals("POST")
                   || context.Request.ContentType != "application/json")
                    return WriteResponseJson(context, new IdentityError
                    {
                        Error = "BadRequest",
                        Description = "The 'ContentType' or Method is not allowed"
                    }, 400);


                _logger.LogInformation("Handling request: " + context.Request.Path);

                if (context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
                    return LogIn(context);
                else
                    return SignIn(context);
            }
            else
                return _next(context);
        }

        private async Task LogIn(HttpContext context)
        {
            var schemaTask = GetJsonSchema("LogIn");
            var jsonTask = GetJson(context);

            var errors = ValidJson(context, await schemaTask, await jsonTask);

            if (errors != null)
            {
                await WriteResponseJson(context, errors, 400);
                return;
            }

            var identity = await _options.IdentityResolver(
               jsonTask.Result.SelectToken("username").Value<string>(),
               jsonTask.Result.SelectToken("password").Value<string>(),
               _userManager);

            var token = GenerateToken(identity, jsonTask.Result.SelectToken("username").Value<string>());

            // Serialize and return the response
            await WriteResponseJson(context, token, 200);
        }

        private async Task SignIn(HttpContext context)
        {
            var schemaTask = GetJsonSchema("SignIn");
            var jsonTask = GetJson(context);

            var errors = ValidJson(context, await schemaTask, await jsonTask);

            if (errors != null)
            {
                await WriteResponseJson(context, errors, 400);
                return;
            }

            var identityResult = await _userManager.CreateAsync(
                new ApplicationUser
                {
                    UserName = jsonTask.Result.SelectToken("username").Value<string>()
                },
                jsonTask.Result.SelectToken("password").Value<string>());

            if (identityResult.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(jsonTask.Result.SelectToken("username").Value<string>());
                var claims = await _userManager.GetClaimsAsync(user);

                var token = GenerateToken(new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"), claims), user.UserName);

                // Serialize and return the response
                await WriteResponseJson(context, token, 200);
            }
            else
            {
                var identityErrors = identityResult.Errors.Select(ir => new IdentityError
                {
                    Description = ir.Description,
                    Error = ir.Code
                }).ToList();
                await WriteResponseJson(context, identityErrors, 404);
            }
        }

        private Task WriteResponseJson(HttpContext context, object json, int code)
        {
            return WriteResponse(context, JsonConvert.SerializeObject(json, new JsonSerializerSettings { Formatting = Formatting.Indented }), "application/json", code);
        }

        private Task WriteResponse(HttpContext context, string buffer, string contentType, int code)
        {
            context.Response.StatusCode = code;
            context.Response.ContentType = contentType;
            return context.Response.WriteAsync(buffer);
        }

        private Token GenerateToken(ClaimsIdentity identity, string sub)
        {
            var now = DateTime.UtcNow;

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
               {
                    new Claim(JwtRegisteredClaimNames.Sub, sub),
                    new Claim(JwtRegisteredClaimNames.Jti, _options.NonceGenerator().Result),
                    new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64)
               };

            identity.AddClaims(claims);

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
               issuer: _options.Issuer,
               audience: _options.Audience,
               claims: identity.Claims,
               notBefore: now,
               expires: now.Add(_options.Expiration),
               signingCredentials: _options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new Token
            {
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds
            };
        }

        private List<IdentityError> ValidJson(HttpContext context, JsonSchema4 schema, JObject json)
        {
            var resultSchema = schema.Validate(json);

            if (resultSchema.Count != 0)
            {

                return resultSchema.Select(r => new IdentityError
                {
                    Description = r.Kind.ToString(),
                    Error = r.Property,
                    LineNumber = r.LineNumber,
                    LinePosition = r.LinePosition
                }).ToList();
            }
            return null;
        }

        private Task<JsonSchema4> GetJsonSchema(string configurationKey)
        {
            var signInSchema = _options.Schemas.GetSection(configurationKey).Value;
            return JsonSchema4.FromJsonAsync(signInSchema);
        }

        private Task<JObject> GetJson(HttpContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var streamReader = new StreamReader(context.Request.Body))
                {
                    return JObject.Parse(streamReader.ReadToEnd());
                }
            });
        }

        private class IdentityError
        {
            public string Description { get; set; }
            public string Error { get; set; }
            public int LineNumber { get; set; }
            public int LinePosition { get; set; }
        }

        private class Token
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }

        private static double ToUnixEpochDate(DateTime now)
        {
            var dateTime = new DateTime(now.Ticks, DateTimeKind.Local);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (dateTime.ToUniversalTime() - epoch).TotalSeconds;
        }

        private static void ThrowIfInvalidOptions(TokenProviderOptions options)
        {
            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Path));
            }

            if (string.IsNullOrEmpty(options.Issuer))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Issuer));
            }

            if (string.IsNullOrEmpty(options.Audience))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Audience));
            }

            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenProviderOptions.Expiration));
            }

            if (options.IdentityResolver == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.IdentityResolver));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.SigningCredentials));
            }

            if (options.NonceGenerator == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.NonceGenerator));
            }

            if (options.SignInPath == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.SignInPath));
            }

            if (options.Schemas == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Schemas));
            }
        }
    }
}
