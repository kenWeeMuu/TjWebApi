using System.Web.Http;
using AutoMapper;
using ErpDb.Entitys.Auth;
using ErpDb.Entitys.Enums;
using WebApi.Extensions;
using WebApi.Manager;
using System.Data.Entity;
using System.Linq;
using ErpDb.Entitys;
using WebApi.Extensions.AuthContext;

namespace WebApi.Controllers.V2
{

 
    [RoutePrefix("api/v2/rbac")]
    public class AccountController : ApiController
    {
        private ErpDbContext _dbContext;
        private IMapper mapper;

        public AccountController(ErpDbContext erpDbContext, IMapper mapper)
        {
            this._dbContext = erpDbContext;
            this.mapper = mapper;
        }

        [Route("account/Profile")]
        [HttpGet]
        public IHttpActionResult Profile()
        {

            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext)
            {
                User user = AuthContextService.CurrentUser;
                var menus = _dbContext.Menus.Where(x =>
                    x.IsDeleted == CommonEnum.IsDeleted.No && x.Status == CommonEnum.Status.Normal).ToList();
                //查询当前登录用户拥有的权限集合(非超级管理员)
                var sqlPermission =
                    @"SELECT P.Code AS PermissionCode,
P.ActionCode AS PermissionActionCode,
P.Name AS PermissionName,
P.Type AS PermissionType,
M.Name AS MenuName,
M.Guid AS MenuId,M.Alias AS MenuAlias,M.IsDefaultRouter 
FROM RolePermission AS RPM 
LEFT JOIN Permission AS P ON P.PermissionId = RPM.Permission_PermissionId
INNER JOIN Menu AS M ON M.MenuId = P.MenuId
WHERE P.IsDeleted=0 AND P.Status=1 
AND EXISTS ( SELECT 1 FROM UserRole AS UR 
				inner join Role as r on Ur.Role_RoleId = r.RoleId and RPM.Role_RoleId = r.RoleId WHERE UR.User_UserId={0} )";
                if (user.UserType == UserType.SuperAdministrator)
                {
                    //如果是超级管理员
                    sqlPermission =
                        @"SELECT P.Code AS PermissionCode,P.ActionCode AS PermissionActionCode,P.Name AS PermissionName,P.Type AS PermissionType,M.Name AS MenuName,M.Guid AS MenuId,M.Alias AS MenuAlias,M.IsDefaultRouter FROM Permission AS P 
INNER JOIN Menu AS M ON M.MenuId = P.MenuId
WHERE P.IsDeleted=0 AND P.Status=1";
                }

                //var queryable = from p in _dbContext.Permissions
                //    from r in p.Roles
                //    where r.Users.Any(u => u.UserId == 4) && p.IsDeleted == 0 && (int) p.Status == 1
                //    select new
                //    {
                //        p.Code,
                //        p.ActionCode,

                //        p.Type,
                //        p.Menu.Name,
                //        p.Menu.MenuId,
                //        p.Menu.Alias,
                //        p.Menu.IsDefaultRouter
                //    };

                var permissions = _dbContext.Database.SqlQuery<PermissionWithMenu>(sqlPermission, user.UserId).ToList();

                var pagePermissions = permissions.GroupBy(x => x.MenuAlias).ToDictionary(g => g.Key, g => g.Select(x => x.PermissionActionCode).Distinct());


                /***
                 *
                 *                path: '/permission',
                                component: Layout,
                                redirect: '/permission/page',
                                alwaysShow: true, // will always show the root menu
                                name: 'Permission',
                                 meta: {
                                 title: 'Permission',
                                 icon: 'lock',
                                 roles: ['admin', 'editor'] // you can set roles in root nav
                                 },
                             children: [
                               {
                                 path: 'page',
                                 component: () => import('@/views/permission/page'),
                                 name: 'PagePermission',
                                 meta: {
                                   title: 'Page Permission',
                                   roles: ['admin'] // or you can only set roles in sub nav
                                 }
                               }
                 *
                 *
                 *
                 *
                 *
                 *
                 *
                 *
                 *
                 *
                 *
                 */



                response.SetData(new
                {
                    roles = new string[] { user.UserType.ToString() },
                    access = new string[] { user.UserType.ToString() },
                    avator = user.Avatar,
                    userId = user.UserId,
                    username = user.DisplayName,
                    user_type = user.UserType,
                    introduction = user.LoginName,
                    permissions = pagePermissions
                });
            }

            return Ok(response);
        }

    }
}