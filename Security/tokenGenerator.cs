using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Elevator.Models;

namespace Elevator.Security
{
    public static class TokenGenerator
    {
        public static string tokenGeneratorJWT(string userName, Jwt_values JWTValues)
        {
            try
            {
                var key = JWTValues.JWT_SET_KEY;
                var audienceToken = JWTValues.JWT_AUDIENCE_TOKEN;
                var IssuerKey = JWTValues.JWT_ISSUER_TOKEN;
                var timeLifeToken = JWTValues.JWT_EXPIRE_MINUTES;

                var SecurityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(key));
                var credencials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userName) });

                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

                var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                    audience: audienceToken,
                    issuer: IssuerKey,
                    subject: claimsIdentity,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(timeLifeToken)),
                    signingCredentials: credencials);

                var strToken = tokenHandler.WriteToken(jwtSecurityToken);

                return strToken;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
