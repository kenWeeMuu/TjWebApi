using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using ErpDb.Entitys.Auth;
using Newtonsoft.Json;
using WebApi.Extensions.AuthContext;
using WebApi.Manager;

namespace WebApi.Filters
{
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Bearer")
                return;

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                return;
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateJwtToken(token);

            if (principal == null)
                context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);

            else
                context.Principal = principal;
        }



        private static bool ValidateToken(string token, out AuthContextUser auser)
        {
            auser = null;

            IDictionary<string, object> simplePrinciple = JwtManager.GetPrincipal(token);

            if (simplePrinciple == null) return false;

            if (simplePrinciple.ContainsKey(ClaimTypes.UserData))
            {
             //   auser = (User) simplePrinciple[ClaimTypes.UserData]  ;
                object _auser;
                if (simplePrinciple.TryGetValue(ClaimTypes.UserData, out _auser))
                {
                    auser = JsonConvert.DeserializeObject<AuthContextUser>(_auser.ToString());
                   
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
             
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            AuthContextUser auser;

            if (ValidateToken(token, out auser))
            {
                // based on username to get more information from database in order to build local identity
                //var claims = new List<Claim>
                //{
                //    new Claim(ClaimTypes.Name, username)
                //    // Add more claims if needed: Roles, ...
                //};

                var identity = new ClaimsIdentity(auser.CreateClaims(), "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);
                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }
    }
}
