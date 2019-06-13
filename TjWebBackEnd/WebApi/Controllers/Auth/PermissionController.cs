using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using ErpDb.Entitys;
using ErpDb.Entitys.Auth;
using ErpDb.Entitys.Enums;
using WebApi.Extensions;
using WebApi.Extensions.AuthContext;
using WebApi.Filters;
using WebApi.QueryModels.DncPermission;
using WebApi.RequestPayload.Rbac.Permission;
using WebApi.Utils;
using WebApi.ViewModel;

namespace WebApi.Controllers.Auth
{
    /// <summary>
    /// 权限控制器
    /// </summary>
    [RoutePrefix("api/v1/rbac")]
    public class PermissionController : ApiController
    {
        private readonly ErpDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public PermissionController(ErpDbContext dbContext, IMapper mapper) {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("permission/list")]
        public IHttpActionResult List(PermissionRequestPayload payload) {
            var response = ResponseModelFactory.CreateResultInstance;
            using (_dbContext) {
                var query = _dbContext.Permissions.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw)) {
                    query = query.Where(x => x.Name.Contains(payload.Kw.Trim()) || x.Code.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > CommonEnum.IsDeleted.All) {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > CommonEnum.Status.All) {
                    query = query.Where(x => x.Status == payload.Status);
                }
                if (payload.MenuId != -5) {
                    query = query.Where(x => x.MenuId == payload.MenuId);
                }
                var list = query.OrderByDescending(x=>x.CreatedOn).Paged(payload.CurrentPage, payload.PageSize).Include(x => x.Menu).ToList();
                var totalCount = query.Count();
                var data = list.Select(_mapper.Map<Permission, PermissionJsonModel>);
                /*
                 * .Select(x => new PermissionJsonModel {
                    MenuName = x.Menu.Name,
                    x.
                });
                 */

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建权限
        /// </summary>
        /// <param name="model">权限视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("permission/create")]
        public IHttpActionResult Create(PermissionCreateViewModel model) {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Name.Trim().Length <= 0) {
                response.SetFailed("请输入权限名称");
                return Ok(response);
            }
            using (_dbContext) {
                if (_dbContext.Permissions.Count(x => x.ActionCode == model.ActionCode && x.MenuId == model.MenuId) > 0) {
                    response.SetFailed("权限操作码已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<PermissionCreateViewModel, Permission>(model);
                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.CreatedByUserId = AuthContextService.CurrentUser.UserId;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;
                _dbContext.Permissions.Add(entity);
                _dbContext.SaveChanges();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑权限
        /// </summary>
        /// <param name="code">权限惟一编码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("permission/edit/{code}")]
        public IHttpActionResult Edit(string code) {
            using (_dbContext) {
                var entity = _dbContext.Permissions.FirstOrDefault(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                var model = _mapper.Map<Permission, PermissionEditViewModel>(entity);
                var menu = _dbContext.Menus.FirstOrDefault(x => x.MenuId == entity.MenuId);
                model.MenuName = menu.Name;
                response.SetData(model);
                return Ok(response);
            }
        }

        /// <summary>
        /// 保存编辑后的权限信息
        /// </summary>
        /// <param name="model">权限视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("permission/edit")]
        public IHttpActionResult Edit(PermissionEditViewModel model) {
            var response = ResponseModelFactory.CreateInstance;
     
            using (_dbContext) {
                if (_dbContext.Permissions.Any(x => x.ActionCode == model.ActionCode && x.Code != model.Code)) {
                    response.SetFailed("权限操作码已存在");
                    return Ok(response);
                }
                var entity = _dbContext.Permissions.FirstOrDefault(x => x.Code == model.Code);
                if (entity == null) {
                    response.SetFailed("权限不存在");
                    return Ok(response);
                }
                entity.Name = model.Name;
                entity.ActionCode = model.ActionCode;
                entity.MenuId = model.MenuId;
                entity.IsDeleted = model.IsDeleted;
                entity.ModifiedByUserId = AuthContextService.CurrentUser.UserId;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;
                entity.ModifiedOn = DateTime.Now;
                entity.Status = model.Status;
                entity.Description = model.Description;
                _dbContext.SaveChanges();
                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="ids">权限ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]

        public IHttpActionResult Delete(string ids) {
            var response = ResponseModelFactory.CreateInstance;
            response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
            return Ok(response);
        }

        /// <summary>
        /// 恢复权限
        /// </summary>
        /// <param name="ids">权限ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        [Route("permission/recover/{ids}")]
        public IHttpActionResult Recover(string ids) {
            var response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">权限ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        [Route("permission/batch/{ids}")]
        public IHttpActionResult Batch(string command, string ids) {
            var response = ResponseModelFactory.CreateInstance;
            switch (command) {
                case "delete":
                 
                    response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
                    break;

                case "recover":
                    response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
                    break;

                case "forbidden":
          
                    response = UpdateStatus(UserStatus.Forbidden, ids);
                    break;

                case "normal":
                    response = UpdateStatus(UserStatus.Normal, ids);
                    break;

                default:
                    break;
            }
            return Ok(response);
        }

        /// <summary>
        /// 角色-权限菜单树
        /// </summary>
        /// <param name="code">角色编码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("permission/permission_tree/{code}")]
        public IHttpActionResult PermissionTree(string code) {
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext) {
                var role = _dbContext.Roles.AsNoTracking().Include(x=>x.Permissions).FirstOrDefault(x => x.Code == code);
                if (role == null) {
                    response.SetFailed("角色不存在");
                    return Ok(response);
                }

              var  menus =  _dbContext.Menus.ToList();
             menus = menus.Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && x.Status == CommonEnum.Status.Normal)
                  .OrderBy(x => x.CreatedOn).ThenBy(x => x.Sort).ToList();

             var menu  =    menus.Select(x => new PermissionMenuTree
                    {
                        Id = x.MenuId,
                        ParentId = x.ParentId,
                        Title = x.Name
                    }).ToList();
                //var menu = _dbContext.Menus.Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && x.Status == CommonEnum.Status.Normal).OrderBy(x => x.CreatedOn).ThenBy(x => x.Sort)
                //    .Select(x => new PermissionMenuTree
                //    {
                //        Id = x.MenuId,
                //        ParentId = x.ParentId,
                //        Title = x.Name
                //    }).ToList();
                //PermissionWithAssignProperty
                var sql = @"    
    SELECT P.Code,P.MenuId,P.Name,P.ActionCode,ISNULL(S.Code,'') AS RoleCode,CASE WHEN S.RoleId IS NOT NULL THEN 1 ELSE 0 END AS IsAssigned 
    FROM dbo.Permission AS P
    LEFT JOIN dbo.RolePermission AS rp ON p.PermissionId = rp.Permission_PermissionId
    LEFT JOIN	dbo.Role AS S ON S.RoleId = rp.Role_RoleId  AND S.Code = '{0}'
    WHERE P.IsDeleted = 0 AND P.Status = 1";
                if (role.IsSuperAdministrator) {
                    sql = @"SELECT  Code, MenuId, Name, ActionCode, CASE WHEN code IS NOT NULL THEN 1  ELSE 0 END  AS IsAssigned	
                                FROM dbo.Permission
                                WHERE IsDeleted=0 AND Status=1";
                }

                sql =string.Format(sql, code);
             //  var _rolesss =  _dbContext.Roles.AsNoTracking().Include(p => p.Permissions)
             //       .Where(r => r.Code == code).ToList();
             //   var _ps = _dbContext.Permissions.AsNoTracking().Include(r => r.Roles.FirstOrDefault(rr=>rr.Code==code)).Where(p =>
             //       p.IsDeleted == CommonEnum.IsDeleted.No && p.Status == CommonEnum.Status.Normal).ToList();
             //var  ress = _ps.SelectMany(s => s.Roles, (p, r) => new PermissionWithAssignProperty(p, r)).ToList();

          

           


             var permissionList = _dbContext.Database.SqlQuery<PermissionWithAssignProperty>(sql).ToList();
                var tree = menu.FillRecursive(permissionList, -1, role.IsSuperAdministrator);
                response.SetData(new { tree, selectedPermissions = permissionList.Where(x => x.IsAssigned == 1).Select(x => x.Code) });
            }

            return Ok(response);
        }

        #region 私有方法

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">权限ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids) {
            using (_dbContext) {
                var parameters = ids.Split(',').Select((id, index) => new SqlParameter($"p{index}", id)).ToList();
                var partsql = string.Join(", ", parameters.Select(p => $"@{p.ParameterName}"));
                var sql = string.Format("UPDATE Permission SET IsDeleted=@IsDeleted WHERE Code IN ({0})", partsql);
                parameters.Add(new SqlParameter("IsDeleted", (int)isDeleted));
                _dbContext.Database.ExecuteSqlCommand(sql, parameters.ToArray());
            
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="status">权限状态</param>
        /// <param name="ids">权限ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateStatus(UserStatus status, string ids) {
            using (_dbContext) {
                var parameters = ids.Split(',').Select((id, index) => new SqlParameter($"p{index}", id)).ToList();
                var sqlJoin = string.Join(", ", parameters.Select(p => $"@{ p.ParameterName}"));
                var sql = $"UPDATE Permission SET Status=@Status WHERE Code IN ({sqlJoin})";
                parameters.Add(new SqlParameter("Status", status));
                _dbContext.Database.ExecuteSqlCommand(sql, parameters.ToArray());
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        #endregion 私有方法
    }

    /// <summary>
    ///
    /// </summary>
    public static class PermissionTreeHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="menus">菜单集合</param>
        /// <param name="permissions">权限集合</param>
        /// <param name="parentGuid">父级菜单GUID</param>
        /// <param name="isSuperAdministrator">是否为超级管理员角色</param>
        /// <returns></returns>
        public static List<PermissionMenuTree> FillRecursive(this List<PermissionMenuTree> menus, List<PermissionWithAssignProperty> permissions, int  parentId, bool isSuperAdministrator = false) {
            List<PermissionMenuTree> recursiveObjects = new List<PermissionMenuTree>();
            foreach (var item in menus.Where(x => x.ParentId == parentId)) {
                var children = new PermissionMenuTree
                {
                    AllAssigned = isSuperAdministrator ? true : (permissions.Where(x => x.MenuId == item.Id).Any(x => x.IsAssigned == 0)),
                    Expand = true,
                    Id = item.Id,
                    ParentId = item.ParentId,
                    Permissions = permissions.Where(x => x.MenuId == item.Id).Select(x => new PermissionElement
                    {
                        Name = x.Name,
                        Code = x.Code,
                        IsAssignedToRole = IsAssigned(x.IsAssigned, isSuperAdministrator)
                    }).ToList(),

                    Title = item.Title,
                    Children = FillRecursive(menus, permissions, item.Id)
                };
                recursiveObjects.Add(children);
            }
            return recursiveObjects;
        }

        private static bool IsAssigned(int isAssigned, bool isSuperAdministrator) {
            if (isSuperAdministrator) {
                return true;
            }
            return isAssigned == 1;
        }

        //public static List<PermissionMenuTree> FillRecursive(this List<PermissionMenuTree> menus, List<PermissionWithAssignProperty> permissions, Guid? parentGuid)
        //{
        //    List<PermissionMenuTree> recursiveObjects = new List<PermissionMenuTree>();

        //    return recursiveObjects;
        //}
    }
}