using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ErpDb.Entitys;
using ErpDb.Entitys.Auth;
using ErpDb.Entitys.Enums;
using WebApi.Extensions;
using WebApi.Extensions.AuthContext;

namespace WebApi.Controllers.V1.Auth
{
    /// <summary>
    /// 
    /// </summary>
    // [EnableCors(origins: "*", headers: "*", methods: "*")]
   // [ApiAuthorize]
   // [JwtAuthentication]
 
    public class AccountController : ApiController
    {
        private readonly ErpDbContext _dbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public AccountController(ErpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //  [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/v1/account/Profile")]
        [HttpGet]
        public IHttpActionResult Profile()
        {
 
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext)
            {
                //TODO





              //  int userid = Convert.ToInt32(((ClaimsPrincipal) HttpContext.Current.User).FindFirst("id").Value);

             //   var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userid);
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

                var pagePermissions = permissions.GroupBy(x => x.MenuAlias)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.PermissionActionCode).Distinct());
                response.SetData(new
                {
                    roles=new string [] { user.UserType.ToString() },
                    access = new string[] { user.UserType.ToString()  },
                    avator = user.Avatar,
                    user_guid = user.UserId,
                    user_name = user.DisplayName,
                    user_type = user.UserType,
                    permissions = pagePermissions

                });
            }

            return Ok(response);
        }

        //        private List<string> FindParentMenuAlias(List<DncMenu> menus, Guid? parentGuid)
        //        {
        //            var pages = new List<string>();
        //            var parent = menus.FirstOrDefault(x => x.Guid == parentGuid);
        //            if (parent != null)
        //            {
        //                if (!pages.Contains(parent.Alias))
        //                {
        //                    pages.Add(parent.Alias);
        //                }
        //                else
        //                {
        //                    return pages;
        //                }
        //
        //                if (parent.ParentGuid != Guid.Empty)
        //                {
        //                    pages.AddRange(FindParentMenuAlias(menus, parent.ParentGuid));
        //                }
        //            }
        //
        //            return pages.Distinct().ToList();
        //        }

        // <summary>
        // 
        // </summary>
        // <returns></returns>

        [Route("api/v1/account/menu")]
        [HttpGet]
        public IHttpActionResult Menu()
        {
            var strSql = @"SELECT M.* FROM RolePermission AS RPM
LEFT JOIN Permission AS P ON P.PermissionId = RPM.Permission_PermissionId
INNER JOIN Menu AS M ON M.MenuId = P.MenuId
WHERE P.IsDeleted= 0 AND P.Status= 1 AND P.Type= 0 AND M.IsDeleted= 0 AND M.Status= 1 
AND EXISTS ( SELECT 1 FROM UserRole AS UR 
				inner join Role as r on Ur.Role_RoleId = r.RoleId and RPM.Role_RoleId = r.RoleId WHERE UR.User_UserId={0} )";
            //TODO


            User user = AuthContextService.CurrentUser;
            if (user.UserType == UserType.SuperAdministrator) {
                //如果是超级管理员
                strSql = @"SELECT * FROM Menu WHERE IsDeleted=0 AND Status=1";
            }
            var menus = _dbContext.Database.SqlQuery<Menu>(strSql, user.UserId).ToList();
            var rootMenus = _dbContext.Menus.Where(x =>
                x.IsDeleted == CommonEnum.IsDeleted.No && x.Status == CommonEnum.Status.Normal &&
                x.ParentId == -1).ToList();
            foreach (var root in rootMenus)
            {
                if (menus.Exists(x => x.MenuId == root.MenuId))
                {
                    continue;
                }

                menus.Add(root);
            }

            var menu = MenuItemHelper.LoadMenuTree(menus,-1);
           var needremoved= menu.Where(w => w.Children.Any() && w.ParentId == -1);
            return Ok(needremoved);
        }
    }


    
    /// <summary>
    /// 
    /// </summary>
    public static class MenuItemHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="selectedGuid"></param>
        /// <returns></returns>
        public static List<MenuItem> BuildTree(this List<MenuItem> menus, int selectedGuid = 0) {
            var lookup = menus.ToLookup(x => x.ParentId);
 
            List<MenuItem> Build(int pid) {
                return lookup[pid]
                    .Select(x => new MenuItem()
                    {
                        MenuItemId=x.MenuItemId,
                        Guid = x.MenuItemId.ToString(),
                        ParentId = x.ParentId,
                        Children = Build(x.MenuItemId),
                        Component = x.Component ?? "Main",
                        Name = x.Name,
                        Path = x.Path,
                        Meta = new MenuMeta
                        {
                            BeforeCloseFun = x.Meta.BeforeCloseFun,
                            HideInMenu = x.Meta.HideInMenu,
                            Icon = x.Meta.Icon,
                            NotCache = x.Meta.NotCache,
                            Title = x.Meta.Title,
                            Permission = x.Meta.Permission
                        }
                    }).ToList();
            }
 
            var result = Build(selectedGuid);
            return result;
        }
 
        public static List<MenuItem> LoadMenuTree(List<Menu> menus, int selectedGuid = 0) {
            var temp = menus.Select(x => new MenuItem
            {
                MenuItemId = x.MenuId,
                ParentId = x.ParentId,
                Name = x.Alias,
                Path = $"/{x.Url}",
                Component = x.Component,
                Meta = new MenuMeta
                {
                    BeforeCloseFun = x.BeforeCloseFun ?? "",
                    HideInMenu = x.HideInMenu == CommonEnum.YesOrNo.Yes,
                    Icon = x.Icon,
                    NotCache = x.NotCache == CommonEnum.YesOrNo.Yes,
                    Title = x.Name
                }
            }).ToList();
            var tree = temp.BuildTree(selectedGuid);
            return tree;
        }
    }
}

 
 
 