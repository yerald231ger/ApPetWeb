﻿using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Encodings.Web;
using AppWebApp.Data;
using Microsoft.AspNetCore.Identity;
using AppWebApp.Models;
using Newtonsoft.Json.Schema;
using NJsonSchema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using System.Security.Principal;

namespace AppWebApp.Services
{
    public class CustomJwtDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string algorithm;
        private readonly TokenValidationParameters validationParameters;

        public CustomJwtDataFormat(string algorithm, TokenValidationParameters validationParameters)
        {
            this.algorithm = algorithm;
            this.validationParameters = validationParameters;
        }

        public AuthenticationTicket Unprotect(string protectedText)
            => Unprotect(protectedText, null);

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            SecurityToken validToken = null;

            try
            {
                principal = handler.ValidateToken(protectedText, this.validationParameters, out validToken);

                var validJwt = validToken as JwtSecurityToken;

                if (validJwt == null)
                {
                    throw new ArgumentException("Invalid JWT");
                }

                if (!validJwt.Header.Alg.Equals(algorithm, StringComparison.Ordinal))
                {
                    throw new ArgumentException($"Algorithm must be '{algorithm}'");
                }

                // Additional custom validation of JWT claims here (if any)
            }
            catch (SecurityTokenValidationException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }

            // Validation passed. Return a valid AuthenticationTicket:
            return new AuthenticationTicket(principal, new AuthenticationProperties(), "Cookie");
        }

        // This ISecureDataFormat implementation is decode-only
        public string Protect(AuthenticationTicket data)
        {
            throw new NotImplementedException();
        }

        public string Protect(AuthenticationTicket data, string purpose)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Provides options for <see cref="TokenProviderMiddleware"/>.
    /// </summary>
    public class TokenProviderOptions
    {
        /// <summary>
        /// The relative request path to listen on.
        /// </summary>
        /// <remarks>The default path is <c>/token</c>.</remarks>
        public string Path { get; set; } = "/token";

        /// <summary>
        ///  The Issuer (iss) claim for generated tokens.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// The Audience (aud) claim for the generated tokens.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// The expiration time for the generated tokens.
        /// </summary>
        /// <remarks>The default is five minutes (300 seconds).</remarks>
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// The signing key to use when generating tokens.
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }

        /// <summary>
        /// Resolves a user identity given a username and password.
        /// </summary>
        public Func<string, string, UserManager<ApplicationUser>, Task<ClaimsIdentity>> IdentityResolver { get; set; }

        /// <summary>
        /// Generates a random value (nonce) for each generated token.
        /// </summary>
        /// <remarks>The default nonce is a random GUID.</remarks>
        public Func<Task<string>> NonceGenerator { get; set; }
            = new Func<Task<string>>(() => Task.FromResult(Guid.NewGuid().ToString()));

        public string SignInPath { get; internal set; }

        public IConfigurationRoot Schemas { get; set; }
    }

    /// <summary>
    /// Token generator middleware component which is added to an HTTP pipeline.
    /// This class is not created by application code directly,
    /// instead it is added by calling the <see cref="TokenProviderAppBuilderExtensions.UseSimpleTokenProvider(Microsoft.AspNetCore.Builder.IApplicationBuilder, TokenProviderOptions)"/>
    /// extension method.
    /// </summary>
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

    public static class TokenProviderAppBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="TokenProviderMiddleware"/> middleware to the specified <see cref="IApplicationBuilder"/>, which enables token generation capabilities.
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="options">A  <see cref="TokenProviderOptions"/> that specifies options for the middleware.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseSimpleTokenProvider(this IApplicationBuilder app, TokenProviderOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));
        }
    }

    //public class MetricsJSchema
    //{
    //    private static readonly Lazy<MetricsJSchema> lazy =
    //        new Lazy<MetricsJSchema>(() => new MetricsJSchema());

    //    public static MetricsJSchema Instance { get { return lazy.Value; } }
    //    public string SJSchema { get; private set; }
    //    public JsonSchema4 JsonSchema4 { get; private set; }

    //    private MetricsJSchema()
    //    {
    //        SJSchema = File.ReadAllText(Configuration);
    //        JsonSchema4 = JsonSchema4.FromJsonAsync(SJSchema).Result;
    //    }

    //    public void UpdateSchema(JObject jObject)
    //    {
    //        SJSchema = jObject.ToString();
    //        File.WriteAllText(ConfigurationManager.AppSettings["json-schema"], SJSchema);
    //        JsonSchema4 = JsonSchema4.FromJsonAsync(SJSchema).Result;
    //    }
    //}
}
