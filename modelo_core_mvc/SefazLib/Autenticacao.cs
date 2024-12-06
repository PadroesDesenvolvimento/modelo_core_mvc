using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Security.Cryptography;

namespace SefazLib;

[ExcludeFromCodeCoverage]
public class Autenticacao
{
    private readonly IConfiguration configuration;
    private readonly string enderecoAuteticador;

    public Autenticacao(IConfiguration Configuration)
    {
        configuration = Configuration;
        enderecoAuteticador = configuration["identity:issuer"];
    }

    public void ConfigureJwtBearerOptions(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["jwt:issuer"],
            ValidAudience = configuration["jwt:audience"],
            IssuerSigningKey = LoadPublicKey()
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var result = JsonConvert.SerializeObject(new { error = "Houve um erro ao validar o token. Verifique se ele está expirado ou tente autenticar-se novamente." });
                return context.Response.WriteAsync(result);
            },
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var result = JsonConvert.SerializeObject(new { error = $"Houve um erro ao autenticar. Verifique se o serviço de autenticação em [{enderecoAuteticador}] está disponível." });
                return context.Response.WriteAsync(result);
            }
        };
    }

    [ExcludeFromCodeCoverage]
    public RsaSecurityKey LoadPublicKey()
    {
        using var client = new HttpClient();
        try
        {
            var publicKeyJwk = client.GetStringAsync(configuration["identity:PublicKey"]).Result;
            var parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(publicKeyJwk);

            var rsaParameters = new RSAParameters
            {
                Modulus = Base64UrlEncoder.DecodeBytes(parameters["n"]),
                Exponent = Base64UrlEncoder.DecodeBytes(parameters["e"])
            };

            var rsa = RSA.Create(2048);
            rsa.ImportParameters(rsaParameters);

            return new RsaSecurityKey(rsa);
        }
        catch (Exception)
        {
        }
        return null;
    }
}