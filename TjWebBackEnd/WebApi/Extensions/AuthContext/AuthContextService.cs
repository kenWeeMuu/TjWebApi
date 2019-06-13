using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Routing;
using ErpDb.Entitys;
using ErpDb.Entitys.Auth;

namespace WebApi.Extensions.AuthContext
{
    public static class AuthContextService
    {
        //todo 此处热修改权限后 会有问题，不过目前这种情况不存在。所以不改先
        public static User CurrentUser {
            get
            {
                //   if (_currentUser == null)
                //   {
                //       var pr = HttpContext.Current.User as ClaimsPrincipal;
                //       var id = pr.FindFirst("id").Value;
                //       _currentUser = currentUser(id);
                //   }

                //return _currentUser;
                var pr = HttpContext.Current.User as ClaimsPrincipal;
                var id = pr.FindFirst("id").Value;
               return currentUser(id);
            }
        }

        private static User _currentUser { get; set; }

        public static bool IsSupperAdministator => UserType.SuperAdministrator == CurrentUser.UserType;


        private static User currentUser(string id)
        {
            using (var db = new ErpDbContext())
            {
               return  db.Users.FirstOrDefault(x => x.UserId.ToString() ==  id );
            }
        }
 

        public static List<Claim> CreateClaims(this AuthContextUser user)
        {
         return   new List<Claim>
            {
                new Claim(JwtClaimTypes.Audience, "api"),
                new Claim(JwtClaimTypes.Issuer, "http://localhost:54321"),
                new Claim(JwtClaimTypes.Id, user.UserId.ToString()),
                new Claim(JwtClaimTypes.Name, user.LoginName),
                new Claim("displayName", user.DisplayName),
                new Claim("loginName", user.LoginName),
                new Claim("avatar", ""),
                new Claim(JwtClaimTypes.Email, ""),
           
                new Claim("userType", ((int) user.UserType).ToString()),
             //   new Claim("r", role),
            };
        }
    }
}