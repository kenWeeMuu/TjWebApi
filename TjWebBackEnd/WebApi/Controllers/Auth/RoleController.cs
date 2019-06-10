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
using WebApi.RequestPayload.Rbac.Role;
using WebApi.Utils;
using WebApi.ViewModel;

namespace WebApi.Controllers.Auth
{
    /// <summary>
    ///
    /// </summary>
 
    [RoutePrefix("api/v1/rbac")]
    public class RoleController : ApiController
    {
        private readonly  ErpDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public RoleController(ErpDbContext dbContext, IMapper mapper) {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("role/list")]
        public IHttpActionResult List(RoleRequestPayload payload) {
            var response = ResponseModelFactory.CreateResultInstance;
            using (_dbContext) {
                var query = _dbContext.Roles.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw)) {
                    query = query.Where(x => x.Name.Contains(payload.Kw.Trim()) || x.Code.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > CommonEnum.IsDeleted.All) {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > CommonEnum.Status.All) {
                    query = query.Where(x => x.Status == payload.Status);
                }
                var list = query.OrderByDescending(x=>x.CreatedOn).Paged(payload.CurrentPage, payload.PageSize).ToList();
                var totalCount = query.Count();
                var data = list.Select(_mapper.Map<Role, RoleJsonModel>);

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="model">角色视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("role/create")]
        public IHttpActionResult Create(RoleCreateViewModel model) {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Name.Trim().Length <= 0) {
                response.SetFailed("请输入角色名称");
                return Ok(response);
            }
            using (_dbContext) {
                if (_dbContext.Roles.Count(x => x.Name == model.Name) > 0) {
                    response.SetFailed("角色已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<RoleCreateViewModel, Role>(model);
                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.IsSuperAdministrator = false;
                entity.IsBuiltin = false;
                entity.CreatedByUserId = AuthContextService.CurrentUser.UserId;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;
                _dbContext.Roles.Add(entity);
                _dbContext.SaveChanges();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="code">角色惟一编码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("role/edit/{code}")]
        public IHttpActionResult Edit(string code) {
            using (_dbContext) {
                var entity = _dbContext.Roles.FirstOrDefault(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                response.SetData(_mapper.Map<Role, RoleCreateViewModel>(entity));
                return Ok(response);
            }
        }

        /// <summary>
        /// 保存编辑后的角色信息
        /// </summary>
        /// <param name="model">角色视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("role/edit")]
        //   [ProducesResponseType(200)]
        public IHttpActionResult Edit(RoleCreateViewModel model) {
            var response = ResponseModelFactory.CreateInstance;
           
            using (_dbContext) {
                if (_dbContext.Roles.Count(x => x.Name == model.Name && x.Code != model.Code) > 0) {
                    response.SetFailed("角色已存在");
                    return Ok(response);
                }

                var entity = _dbContext.Roles.FirstOrDefault(x => x.Code == model.Code);

                if (entity.IsSuperAdministrator && !AuthContextService.IsSupperAdministator) {
                    response.SetFailed("没有足够的权限");
                    return Ok(response);
                }

                entity.Name = model.Name;
                entity.IsDeleted = model.IsDeleted;
                entity.ModifiedByUserId = AuthContextService.CurrentUser.UserId;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;
                entity.ModifiedOn = DateTime.Now;
                entity.Status = model.Status;
                entity.Description = model.Description;
                _dbContext.SaveChanges();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="ids">角色ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        [Route("role/delete/{ids}")]
        public IHttpActionResult Delete(string ids) {
            var response = ResponseModelFactory.CreateInstance;
            response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
            return Ok(response);
        }

        /// <summary>
        /// 恢复角色
        /// </summary>
        /// <param name="ids">角色ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        [Route("role/reover/{ids}")]
        public IHttpActionResult Recover(string ids) {
            var response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">角色ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        [Route("role/batch")]
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
        /// 为指定角色分配权限
        /// </summary>
        /// <param name="payload">角色分配权限的请求载体类</param>
        /// <returns></returns>
        [HttpPost]
        [Route("role/assign_permission")]
        public IHttpActionResult AssignPermission(RoleAssignPermissionPayload payload) {
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext) {
                var role = _dbContext.Roles.Include(x=>x.Permissions).FirstOrDefault(x => x.Code == payload.RoleCode);
                if (role == null) {
                    response.SetFailed("角色不存在");
                    return Ok(response);
                }
                // 如果是超级管理员，则不写入到角色-权限映射表(在读取时跳过角色权限映射，直接读取系统所有的权限)
                if (role.IsSuperAdministrator) {
                    response.SetSuccess();
                    return Ok(response);
                }
                //先删除当前角色原来已分配的权限
                // _dbContext.Database.ExecuteSqlCommand("DELETE FROM DncRolePermissionMapping WHERE RoleCode={0}", payload.RoleCode);
                role.Permissions.Clear();
                _dbContext.SaveChanges();
                if (payload.Permissions != null || payload.Permissions.Count > 0) {
                var permissions =    _dbContext.Permissions.Where(x => payload.Permissions.Contains(x.Code));
                    role.Permissions.CopyTo(permissions.ToArray(),0);
                    _dbContext.SaveChanges();
                }
            }
            return Ok(response);
        }

        [Route("role/find_list_by_user_userId/{userId}")]
        [HttpGet]
        public IHttpActionResult FindListByUserGuid(int userId) {
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext) {
                //有N+1次查询的性能问题
                //var query = _dbContext.User
                //    .Include(r => r.UserRoles)
                //    .ThenInclude(x => x.Role)
                //    .Where(x => x.Guid == guid);
                //var roles = query.FirstOrDefault().UserRoles.Select(x => new
                //{
                //    x.Role.Code,
                //    x.Role.Name
                //});
                var sql = @"SELECT R.* FROM UserRoleMapping AS URM
INNER JOIN Role AS R ON R.Code=URM.RoleCode
WHERE URM.UserGuid={0}";

                var query = _dbContext.Users.Include(x => x.Roles).Single(x => x.UserId == userId).Roles.ToList();


                //var query = _dbContext.Role.FromSql(sql, guid).ToList();
                var assignedRoles = query.ToList().Select(x => x.Code).ToList();
                var roles = _dbContext.Roles.Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && x.Status == CommonEnum.Status.Normal).ToList().Select(x => new { label = x.Name, key = x.Code });
                response.SetData(new { roles, assignedRoles });
                return Ok(response);
            }
        }

        /// <summary>
        /// 查询所有角色列表(只包含主要的字段信息:name,code)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("role/find_simple_list")]
        public IHttpActionResult FindSimpleList() {
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext) {
                var roles = _dbContext.Roles.Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && x.Status == CommonEnum.Status.Normal).Select(x => new { x.Name, x.Code }).ToList();
                response.SetData(roles);
            }
            return Ok(response);
        }

        #region 私有方法

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">角色ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids) {
            using (_dbContext)
            {
                var parames = new List<SqlParameter>();
                var parameters = ids.Split(',').Select((id, index) => new SqlParameter($"p{index}", id)).ToArray();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = $"UPDATE [Role] SET IsDeleted=@IsDeleted WHERE Code IN ({ string.Join(", ", parameters.Select((id, index) =>$"@p{index}"))})";
               // parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
               var param = new SqlParameter("IsDeleted", (int) isDeleted);
               parames.AddRange(parameters);
               parames.Add(param);
                _dbContext.Database.ExecuteSqlCommand(sql, parames.ToArray());
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="status">角色状态</param>
        /// <param name="ids">角色ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateStatus(UserStatus status, string ids) {
            using (_dbContext) {
                var parames = new List<SqlParameter>();
                var parameters = ids.Split(',').Select((id, index) => new SqlParameter($"p{index}", id)).ToArray();
                var parameterNames = string.Join(", ", parameters.Select(p => $"@{p.ParameterName}"));
                var sql = $"UPDATE [Role] SET Status=@Status WHERE Code IN ({parameterNames})";
            //    parameters.Add(new SqlParameter("@Status", (int)status));
            var param = new SqlParameter("Status", (int) status);
            parames.AddRange(parameters);
            parames.Add(param);
                _dbContext.Database.ExecuteSqlCommand(sql, parames.ToArray());
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        #endregion 私有方法
    }
}