using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SefazLib.SamlToJwt
{
    public class SamlToJwt
    {
        public static string ConvertSamlToJwt(string samlToken, IConfiguration configuration)
        {
            var issuer = configuration["identity:metadataaddress"];
            var audience = configuration["identity:realm"];

            var privateKey = configuration["identity:PrivateKey"];
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new RsaSecurityKey(rsa);

            var samlTokenXml = new XmlDocument();
            samlTokenXml.LoadXml(samlToken);

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = issuer,
                    Audience = audience,
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
                };

                // Extrai as reivindicações do token SAML
                var claims = new ClaimsIdentity();
                var samlAssertionNode = samlTokenXml.SelectSingleNode("//*[local-name()='Assertion']");
                var samlAttributeNodes = samlAssertionNode.SelectNodes("//*[local-name()='Attribute']");

                foreach (XmlNode attributeNode in samlAttributeNodes)
                {
                    var attributeName = attributeNode.Attributes["Name"].Value;
                    var attributeValue = attributeNode.FirstChild.InnerText;
                    claims.AddClaim(new Claim(attributeName, attributeValue));
                }

                tokenDescriptor.Subject = claims;
                var signedToken = tokenHandler.CreateToken(tokenDescriptor);

                return handler.WriteToken(signedToken);
            }
            catch
            {
                return samlTokenXml.InnerText;
            }
        }
    }
}
