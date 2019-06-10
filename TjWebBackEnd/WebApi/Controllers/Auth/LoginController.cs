using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using ErpDb.Entitys;
using ErpDb.Entitys.Auth;
using ErpDb.Entitys.Enums;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using WebApi.Extensions;
using WebApi.Manager;

namespace WebApi.Controllers.Auth
{
    [AllowAnonymous]
    // [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        private readonly ErpDbContext _dbContext;

        public LoginController()
        {
            _dbContext = new ErpDbContext();
        }

        [Route("api/Oauth/Auth")]
        [HttpGet]
        public IHttpActionResult aaa(string username, string password)
        {
            var response = ResponseModelFactory.CreateInstance;
            User user;
            using (_dbContext)
            {
                user = _dbContext.Users.Include(x=>x.Roles).FirstOrDefault(x => x.LoginName == username.Trim());

                if (user == null)
                {
                    response.SetFailed("出现未知错误!");
                    return Ok(response);
                }

                if (user == null || user.IsDeleted == CommonEnum.IsDeleted.Yes)
                {
                    response.SetFailed("用户不存在");
                    return Ok(response);
                }

                if (user.Password != password.Trim())
                {
                    response.SetFailed("密码不正确");
                    return Ok(response);
                }

                if (user.IsLocked == CommonEnum.IsLocked.Locked)
                {
                    response.SetFailed("账号已被锁定");
                    return Ok(response);
                }

                if (user.Status == UserStatus.Forbidden)
                {
                    response.SetFailed("账号已被禁用");
                    return Ok(response);
                }

              var token =  JwtManager.GenerateToken(user);



            
                    response.SetData(token);
                    return Ok(response);
            }
        }


        private string getRoles(string guid) {
            if (string.IsNullOrEmpty(guid)) return string.Empty;

            var sql = @"SELECT R.* FROM DncUserRoleMapping AS URM
INNER JOIN DncRole AS R ON R.Code=URM.RoleCode
WHERE URM.UserGuid=@p0";

            
            return _dbContext.Roles.SqlQuery(sql, guid).Select(x => x.Name).FirstOrDefault();
            //  return query.Any(any => any.Name == "super_adminstrator");
        }
    }
}
