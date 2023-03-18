using Elevator.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elevator.Security
{
    public static class tokenValidation
    {
        /// <summary>
        ///     Validate Token
        /// </summary>
        /// <param name="token">Security token</param>
        /// <returns>
        ///     true or false
        /// </returns>
        public static bool Validation(string token, Jwt_values JWTValues)
        {
            try
            {
                var key = JWTValues.JWT_SET_KEY;
                var audienceToken = JWTValues.JWT_AUDIENCE_TOKEN;
                var IssuerKey = JWTValues.JWT_ISSUER_TOKEN;
                var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(key));

                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    ValidAudience = audienceToken,
                    ValidIssuer = IssuerKey,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = LifetimeValidator,
                    IssuerSigningKey = securityKey
                };

                // Extract and assign Current Principal
                Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

                if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SecurityTokenValidationException se)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        ///     Valida el tiempo de vida del token
        /// </summary>
        /// <param name="notBefore"></param>
        /// <param name="expires"></param>
        /// <param name="securityToken"></param>
        /// <param name="validationParameters"></param>
        /// <returns>
        ///     true or false
        /// </returns>
        public static bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;
        }
    }
}
