using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
            var clientSecret = configuration["identity:ClientSecret"]; 

            var samlTokenXml = new XmlDocument();
            samlTokenXml.LoadXml(samlToken);

            if (!string.IsNullOrEmpty(clientSecret))
            {
                var handler = new JwtSecurityTokenHandler();
                try
                {
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Issuer = issuer,
                        Audience = audience,
                        Expires = DateTime.UtcNow.AddMinutes(60),
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clientSecret)), 
                            SecurityAlgorithms.HmacSha256Signature)
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

                    var jwtToken = handler.CreateJwtSecurityToken(tokenDescriptor);

                    return handler.WriteToken(jwtToken);
                }
                catch
                {
                    return samlTokenXml.InnerText;
                }
            }
            else
            {
                return "ClientSecret not provided."; // Tratar a situação em que o ClientSecret é nulo ou vazio
            }
        }
    }
}
