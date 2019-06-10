using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using ErpDb.Entitys.Auth;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using WebApi.Extensions.AuthContext;

namespace WebApi.Manager
{
    public static class JwtManager
    {
        private static readonly string Secret = ConfigurationManager.AppSettings["Secret"];

        private static IJwtEncoder encoder
        {
            get
            {
                if (_encoder == null)
                {
                    IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                    IJsonSerializer serializer = new JsonNetSerializer();
                    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                    _encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
                }

                return _encoder;
            }
        }

        private static IJwtEncoder _encoder { get; set; }

        private static IJwtDecoder _decoder { get; set; }

        private static IJwtDecoder decoder
        {
            get
            {
                if (_decoder == null)
                {
                    IJsonSerializer serializer = new JsonNetSerializer();
                    IDateTimeProvider provider = new UtcDateTimeProvider();
                    IJwtValidator validator = new JwtValidator(serializer, provider);
                    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                    _decoder = new JwtDecoder(serializer, validator, urlEncoder);
                }

                return _decoder;
            }
        }

        public static string GenerateToken(User user, int expireMinutes = 20)
        {
            IDateTimeProvider provider = new UtcDateTimeProvider();
            var now = provider.GetNow().AddMinutes(expireMinutes);
            var secondsSinceEpoch = Math.Round((now - UnixEpoch.Value).TotalSeconds);
           AuthContextUser auser = new AuthContextUser(user);
            var payload = new Dictionary<string, object>
            {
                {ClaimTypes.Name, user.LoginName},
                { ClaimTypes.UserData, auser},
                {"exp", secondsSinceEpoch}
            };

            var token = encoder.Encode(payload, Secret);

            return token;
        }

        public static IDictionary<string, object> GetPrincipal(string token)
        {
            try
            {
                IDictionary<string, object> json = decoder.DecodeToObject(token, Secret, verify: true);
                return json;
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
            }

            return null;
        }

        //        public static string GenerateToken(string username, int expireMinutes = 20)
        //        {
        //            var symmetricKey = Convert.FromBase64String(Secret);
        //            tokenhandler.SetDefaultTimesOnTokenCreation = false;
        //            var now = DateTime.UtcNow;
        //            var nbf = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
        //            var exp = ((DateTime.Now.AddSeconds(15).ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
        //
        //         
        //            var tokenDescriptor = new SecurityTokenDescriptor
        //            {
        //                Subject = new ClaimsIdentity(new[]
        //                        {
        //                            new Claim(ClaimTypes.Name, username),
        //                            new Claim("nbf",nbf ),
        //                            new Claim("exp", exp),
        //                            new Claim("iat", nbf),
        //                        }),
        //                 Expires = DateTime.Now.AddSeconds(15),
        //          //   Expires=DateTime.Now.AddSeconds(12),
        //            //  NotBefore= new DateTimeOffset(DateTime.UtcNow).DateTime,
        //                 NotBefore= DateTime.Now,
        //
        //                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature),
        //            };
        //    
        //            var stoken = tokenhandler.CreateToken(tokenDescriptor);
        //            var token = tokenhandler.WriteToken(stoken);
        //
        //            return token;
        //        }

        public static object x203 { get; set; }

//        public static ClaimsPrincipal GetPrincipal(string token)
//        {
//            try
//            {
//               
//                var jwtToken = tokenhandler.ReadToken(token) as JwtSecurityToken;
//
//                if (jwtToken == null)
//                    return null;
//
//                var symmetricKey = Convert.FromBase64String(Secret);
//
//                var validationParameters = new TokenValidationParameters()
//                {
//                     ValidateLifetime = true,
//                   RequireExpirationTime = true,
//                   ValidateIssuer = false,
//                   ValidateAudience = false,
//                   IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
//                };
//
//                SecurityToken securityToken;
//                
//                var principal = tokenhandler.ValidateToken(token, validationParameters, out securityToken);
//
//                return principal;
//            }
//
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//                return null;
//            }
//        }
    }
}