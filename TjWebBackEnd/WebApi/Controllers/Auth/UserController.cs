using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ErpDb.Entitys;
using ErpDb.Entitys.Auth;
using ErpDb.Entitys.Enums;
using WebApi.Extensions;
using WebApi.Extensions.AuthContext;
using WebApi.RequestPayload.Rbac.User;
using WebApi.ViewModel;

namespace WebApi.Controllers.Auth
{
    /// <summary>
    ///
    /// </summary>
    [RoutePrefix("api/v1/rbac")]
    public class UserController : ApiController
    {
        private readonly ErpDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public UserController(ErpDbContext dbContext, IMapper mapper) {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        ///
        /// </summary>
        [Route("user/list")]
        [HttpPost]
        public IHttpActionResult List(UserRequestPayload payload) {
            using (_dbContext) {
                 var query = _dbContext.Users.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw)) {
                    query = query.Where(x => x.LoginName.Contains(payload.Kw.Trim()) || x.DisplayName.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > CommonEnum.IsDeleted.All) {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > UserStatus.All) {
                    query = query.Where(x => x.Status == payload.Status);
                }

                if (payload.FirstSort != null) {
                    query = query.OrderBy(payload.FirstSort.Field, payload.FirstSort.Direct == "DESC" );
                }
                var list = query.Paged(payload.CurrentPage, payload.PageSize).ToList();
                var totalCount = query.Count();
                var data = list.Select(_mapper.Map<User, UserJsonModel>);
                var response = ResponseModelFactory.CreateResultInstance;
                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="model">用户视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("user/create")]
      //  [ProducesResponseType(200)]
        public IHttpActionResult Create(UserCreateViewModel model) {
            var response = ResponseModelFactory.CreateInstance;
            if (model.LoginName.Trim().Length <= 0) {
                response.SetFailed("请输入登录名称");
                return Ok(response);
            }
            using (_dbContext) {
                if (_dbContext.Users.Count(x => x.LoginName == model.LoginName) > 0) {
                    response.SetFailed("登录名已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<UserCreateViewModel, User>(model);
                entity.CreatedOn = DateTime.Now;
                entity.Status = model.Status;
                entity.CreatedByUserId = AuthContextService.CurrentUser.UserId;
                _dbContext.Users.Add(entity);
                _dbContext.SaveChanges();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns></returns>
        [HttpGet]
        [Route("user/edit/{userId:int}")]
      //  [ProducesResponseType(200)]
        public IHttpActionResult Edit(int userId) {
            using (_dbContext) {
                var entity = _dbContext.Users.FirstOrDefault(x => x.UserId == userId);
                var response = ResponseModelFactory.CreateInstance;
                response.SetData(_mapper.Map<User, UserEditViewModel>(entity));
                return Ok(response);
            }
        }

        /// <summary>
        /// 保存编辑后的用户信息
        /// </summary>
        /// <param name="model">用户视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("user/edit")]
      //  [ProducesResponseType(200)]
        public IHttpActionResult Edit(UserEditViewModel model) {
            var response = ResponseModelFactory.CreateInstance;
       
            using (_dbContext) {
                var entity = _dbContext.Users.FirstOrDefault(x => x.UserId == model.UserId);
                if (entity == null) {
                    response.SetFailed("用户不存在");
                    return Ok(response);
                }

                entity.LoginName = model.LoginName;
                entity.DisplayName = model.DisplayName;
                entity.IsDeleted = model.IsDeleted;
                entity.IsLocked = model.IsLocked;
                entity.ModifiedByUserId = AuthContextService.CurrentUser.UserId;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;
                entity.ModifiedOn = DateTime.Now;
                entity.Password = model.Password;
                entity.Status = model.Status;
                entity.UserType = model.UserType;
                entity.Description = model.Description;
                _dbContext.SaveChanges();
                response = ResponseModelFactory.CreateInstance;
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="ids">用户GUID,多个以逗号分隔</param>
        /// <returns></returns>
         [HttpGet ]
            [Route("user/delete/{ids:int}")]
    //    [ProducesResponseType(200)]
        public IHttpActionResult Delete(int ids) {
            var response = ResponseModelFactory.CreateInstance;
           
            response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids.ToString());
            return Ok(response);
        }

        /// <summary>
        /// 恢复用户
        /// </summary>
        /// <param name="ids">用户GUID,多个以逗号分隔</param>
        /// <returns></returns>
     //   [HttpGet("{ids}")]
     //   [ProducesResponseType(200)]
        public IHttpActionResult Recover(string ids) {
            var response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">用户ID,多个以逗号分隔</param>
        /// <returns></returns>
           [HttpGet]
            [Route("user/batch")]
     //   [ProducesResponseType(200)]
        public IHttpActionResult Batch(string command, string ids) {
            var response = ResponseModelFactory.CreateInstance;
            switch (command) {
                case "delete":
//                    if (ConfigurationManager.AppSettings.IsTrialVersion) {
//                        response.SetIsTrial();
//                        return Ok(response);
//                    }
                    response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
                    break;

                case "recover":
                    response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
                    break;

                case "forbidden":
//                    if (ConfigurationManager.AppSettings.IsTrialVersion) {
//                        response.SetIsTrial();
//                        return Ok(response);
//                    }
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

        #region 用户-角色

        /// <summary>
        /// 保存用户-角色的关系映射数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(("user/save_roles"))]
        public IHttpActionResult SaveRoles(SaveUserRolesViewModel model) {
            var response = ResponseModelFactory.CreateInstance;
            var user = _dbContext.Users.Include(x => x.Roles).Single(u => u.UserId == model.UserId);
            user.Roles.Clear();
            _dbContext.SaveChanges();
            var success = true;

            if (model.AssignedRoles.Count > 0) {

              
               var roles = _dbContext.Roles.Where(r => model.AssignedRoles.Contains(r.Code));
               foreach (Role role in roles)
               {
                   user.Roles.Add(role);
               }
                success = _dbContext.SaveChanges() > 0;
            }

            if (success) {
                response.SetSuccess();
            }
            else {
                response.SetFailed("保存用户角色数据失败");
            }
            return Ok(response);
        }


      
        #endregion 用户-角色

        #region 私有方法

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">用户ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids) {
            using (_dbContext) {
                string _sqlpart = string.Empty;
                    
                _sqlpart = string.Join(", ", ids.Split(',').Select((id, index) => $"@p{index}"));
                var parameters = ids.Split(',').Select((id, index) => new SqlParameter($"p{index}", int.Parse(id))).ToArray();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = string.Format($"UPDATE [User] SET IsDeleted={(int)isDeleted} WHERE userId IN ({_sqlpart})");
            //    var sql = string.Format($"UPDATE [User] SET IsDeleted=@IsDeleted WHERE userId IN ({_sqlpart})");
                 //   , new SqlParameter("IsDeleted", (int)isDeleted)
                _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="status">用户状态</param>
        /// <param name="ids">用户ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateStatus(UserStatus status, string ids) {
            using (_dbContext)
            {
          //      var _idList = ids.Split(',').Select((id, index) => $"@p{index}").ToList();
                string _sqlpart = string.Empty;
//                _idList.ForEach(id=> _sqlpart+=id+",");
//               var endindex = _sqlpart.LastIndexOf(",");
//               if (endindex > 0) _sqlpart = _sqlpart.Substring(0, endindex);
               _sqlpart= string.Join(", ", ids.Split(',').Select((id, index) => $"@p{index}"));

                var parameters = ids.Split(',').Select((id, index) => new SqlParameter($"p{index}", int.Parse(id))).ToArray();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = string.Format($"UPDATE [User] SET Status={((int)status)} WHERE userId IN ({_sqlpart})");
                //parameters.Add(new SqlParameter("Status", (int)status));
                _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        #endregion 私有方法
    }
}