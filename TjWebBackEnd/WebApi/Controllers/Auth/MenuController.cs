using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using ErpDb.Entitys;
using ErpDb.Entitys.Enums;
using WebApi.Extensions;
using WebApi.Filters;
using WebApi.ViewModel;

namespace WebApi.Controllers.Auth
{
    [Route("api/v1/rbac/[controller]/[action]")]
    [JwtAuthentication]
    public class MenuController : ApiController
    {
        private readonly ErpDbContext _dbContext;


        /// <summary>
        ///
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public MenuController()
        {
            _dbContext = new ErpDbContext();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/rbac/menu/list")]
        public IHttpActionResult List(MenuRequestPayload payload)
        {
            using (_dbContext)
            {
                var query = _dbContext.Menus.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.Name.Contains(payload.Kw.Trim()) || x.Url.Contains(payload.Kw.Trim()));
                }

                if (payload.IsDeleted > CommonEnum.IsDeleted.All)
                {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }

                if (payload.Status > CommonEnum.Status.All)
                {
                    query = query.Where(x => x.Status == payload.Status);
                }

                //-5代表全部都差
                if (payload.ParentId != -5)
                {
                    query = query.Where(x => x.ParentId == payload.ParentId);
                }

                var list = query.OrderBy(x => x.MenuId).Paged(payload.CurrentPage, payload.PageSize).ToList();
                var totalCount = query.Count();
                // var data = list.Select(_mapper.Map<DncMenu, MenuJsonModel>);
                var response = ResponseModelFactory.CreateResultInstance;
                response.SetData(list, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="model">菜单视图实体</param>
        /// <returns></returns>
        //   [HttpPost]
        ////   [ProducesResponseType(200)]
        //   public IHttpActionResult Create(MenuCreateViewModel model) {
        //       using (_dbContext) {
        //           var entity = _mapper.Map<MenuCreateViewModel, DncMenu>(model);
        //           entity.CreatedOn = DateTime.Now;
        //           entity.Guid = Guid.NewGuid();
        //           entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
        //           entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;
        //           _dbContext.DncMenu.Add(entity);
        //           _dbContext.SaveChanges();
        //           var response = ResponseModelFactory.CreateInstance;
        //           response.SetSuccess();
        //           return Ok(response);
        //       }
        //   }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        /// <param name="guid">菜单ID</param>
        /// <returns></returns>
        [Route("api/v1/rbac/menu/edit/{menuId}")]
        [HttpGet]
        //[ProducesResponseType(200)]
        public IHttpActionResult Edit(int menuId)
        {
            using (_dbContext)
            {
                var entity = _dbContext.Menus.FirstOrDefault(x => x.MenuId == menuId);
                var response = ResponseModelFactory.CreateInstance;


                var tree = LoadMenuTree(entity.ParentId);
                response.SetData(new {entity, tree});
                return Ok(response);
            }
        }

        [Route("api/v1/rbac/menu/edit")]
        [HttpPost]
        public IHttpActionResult Edit(MenuEditViewModel model)
        {
            using (_dbContext)
            {
                var entity = _dbContext.Menus.FirstOrDefault(x => x.MenuId == model.MenuId);
                entity.Name = model.Name;
                entity.Icon = model.Icon;
                entity.Level = 1;
                entity.ParentId = model.ParentId;
                entity.Sort = model.Sort;
                entity.Url = model.Url;
                //todo 添加方便的获取用户id方法
                entity.ModifiedByUserId = 1;
                entity.ModifiedByUserName = "super_administrator";
                entity.ModifiedOn = DateTime.Now;
                entity.Description = model.Description;
                entity.ParentName = model.ParentName;
                entity.Component = model.Component;
                entity.HideInMenu = model.HideInMenu;
                entity.NotCache = model.NotCache;
                entity.BeforeCloseFun = model.BeforeCloseFun;
                entity.Alias = model.Alias;
                entity.IsDeleted = model.IsDeleted;
                entity.Status = model.Status;
                entity.IsDefaultRouter = model.IsDefaultRouter;
                _dbContext.SaveChanges();
                var response = ResponseModelFactory.CreateInstance;
                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 菜单树
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/rbac/menu/tree/{selected}")]
      //  [Route("api/v1/rbac/menu/tree/{selected:int=-1}")]
        [HttpGet]
        public IHttpActionResult Tree(int selected)
        {
            var response = ResponseModelFactory.CreateInstance;
            var tree = LoadMenuTree(selected);
            response.SetData(tree);
            return Ok(response);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="ids">菜单ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/rbac/menu/delete/{ids}")]
        public IHttpActionResult Delete(string ids) {
            var response = ResponseModelFactory.CreateInstance;
      
            response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
            return Ok(response);
        }

        //   /// <summary>
        //   /// 恢复菜单
        //   /// </summary>
        //   /// <param name="ids">菜单ID,多个以逗号分隔</param>
        //   /// <returns></returns>
        //   [ HttpGet("{ids}")]
        //   [ProducesResponseType(200)]
        //   public IActionResult Recover(string ids) {
        //       var response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
        //       return Ok(response);
        //   }

        //   /// <summary>
        //   /// 批量操作
        //   /// </summary>
        //   /// <param name="command"></param>
        //   /// <param name="ids">菜单ID,多个以逗号分隔</param>
        //   /// <returns></returns>
        //   [System.Web.Mvc.HttpGet]
        //   [ProducesResponseType(200)]
        //   public IHttpActionResult Batch(string command, string ids) {
        //       var response = ResponseModelFactory.CreateInstance;
        //       switch (command) {
        //           case "delete":
        //               if (ConfigurationManager.AppSettings.IsTrialVersion) {
        //                   response.SetIsTrial();
        //                   return Ok(response);
        //               }
        //               response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
        //               break;

        //           case "recover":
        //               response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
        //               break;

        //           case "forbidden":
        //               if (ConfigurationManager.AppSettings.IsTrialVersion) {
        //                   response.SetIsTrial();
        //                   return Ok(response);
        //               }
        //               response = UpdateStatus(UserStatus.Forbidden, ids);
        //               break;

        //           case "normal":
        //               response = UpdateStatus(UserStatus.Normal, ids);
        //               break;

        //           default:
        //               break;
        //       }

        //       return Ok(response);
        //   }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">菜单ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids) {
            using (_dbContext) {
                var parameters = ids.Split(',').Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = string.Format("UPDATE DncMenu SET IsDeleted=@IsDeleted WHERE meenuId IN ({0})", parameterNames);
                parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
                _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        //   /// <summary>
        //   /// 删除菜单
        //   /// </summary>
        //   /// <param name="status">菜单状态</param>
        //   /// <param name="ids">菜单ID字符串,多个以逗号隔开</param>
        //   /// <returns></returns>
        //   private ResponseModel UpdateStatus(UserStatus status, string ids) {
        //       using (_dbContext) {
        //           var parameters = ids.Split(',').Select((id, index) => new SqlParameter($"@p{index}", id)).ToList();
        //           var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
        //           var sql = string.Format("UPDATE DncMenu SET Status=@Status WHERE Guid IN ({0})", parameterNames);
        //           parameters.Add(new SqlParameter("@Status", (int)status));
        //           _dbContext.Database.ExecuteSqlCommand(sql, parameters);
        //           var response = ResponseModelFactory.CreateInstance;
        //           return response;
        //       }
        //   }

        private List<MenuTree> LoadMenuTree(int selectedGuid = -1)
        {
            var temp = _dbContext.Menus
                .Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && x.Status == CommonEnum.Status.Normal).ToList()
                .Select(x => new MenuTree
                {
                    MenuId = x.MenuId,
                    Guid = x.Guid.ToString(),
                    ParentId = x.ParentId,
                    Title = x.Name
                }).ToList();
            var root = new MenuTree
            {
                Title = "顶级菜单",
                Guid = Guid.Empty.ToString(),
                ParentId = -3,
                MenuId = -1
            };
            temp.Insert(0, root);
            var tree = temp.BuildTree(selectedGuid);
            return tree;
        }
    }


    public static class MenuTreeHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="selectedGuid"></param>
        /// <returns></returns>
        public static List<MenuTree> BuildTree(this List<MenuTree> menus, int selectedGuid = -1)
        {
            var lookup = menus.ToLookup(x => x.ParentId);
            Func<int, List<MenuTree>> build = null;
            build = pid =>
            {
                var _LIST = lookup[pid]
                    .Select(x => new MenuTree()
                    {
                        MenuId = x.MenuId,
                        Guid = x.Guid,
                        ParentId = x.ParentId,
                        Title = x.Title,
                        Expand = x.ParentId == -3,
                        Selected = selectedGuid == x.MenuId,
                        Children = build(x.MenuId),
                    })
                    .ToList();


                return _LIST;
            };
            var result = build(-3);
            return result;
        }
    }
}