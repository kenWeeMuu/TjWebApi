using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using ErpDb.Entitys;
using ErpDb.Entitys.Auth;
using ErpDb.Entitys.Enums;
using WebApi.Extensions;
using WebApi.Extensions.AuthContext;
using WebApi.RequestPayload.Rbac.Icon;
using WebApi.ViewModel;

namespace WebApi.Controllers.Auth
{
    [RoutePrefix("api/v1/rbac")]
    public class IconController : ApiController
    {
        private readonly ErpDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public IconController(ErpDbContext dbContext, IMapper mapper) {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("icon/list")]
        public IHttpActionResult List(IconRequestPayload payload) {
            using (_dbContext) {
                var query = _dbContext.Icons.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw)) {
                    query = query.Where(x => x.Code.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > CommonEnum.IsDeleted.All) {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > CommonEnum.Status.All) {
                    query = query.Where(x => x.Status == payload.Status);
                }
                var list = query.OrderByDescending(o=>o.Id).Paged(payload.CurrentPage, payload.PageSize).ToList();
                var totalCount = query.Count();
                var data = list.Select(_mapper.Map<Icon, IconJsonModel>);
                var response = ResponseModelFactory.CreateResultInstance;
                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("icon/find_list_by_kw/{kw}")]
        public IHttpActionResult FindByKeyword(string kw) {
            var response = ResponseModelFactory.CreateResultInstance;
            if (string.IsNullOrEmpty(kw)) {
                response.SetFailed("没有查询到数据");
                return Ok(response);
            }
            using (_dbContext) {
                var query = _dbContext.Icons.Where(x => x.Code.Contains(kw));

                var list = query.ToList();
                var data = list.Select(x => new { x.Code, x.Color, x.Size });

                response.SetData(data);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建图标
        /// </summary>
        /// <param name="model">图标视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public IHttpActionResult Create(IconCreateViewModel model) {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Code.Trim().Length <= 0) {
                response.SetFailed("请输入图标名称");
                return Ok(response);
            }
            using (_dbContext) {
                if (_dbContext.Icons.Count(x => x.Code == model.Code) > 0) {
                    response.SetFailed("图标已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<IconCreateViewModel, Icon>(model);
                entity.CreatedOn = DateTime.Now;
                entity.CreatedByUserId = AuthContextService.CurrentUser.UserId;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;
                _dbContext.Icons.Add(entity);
                _dbContext.SaveChanges();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑图标
        /// </summary>
        /// <param name="id">图标ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("icon/edit/{id}")]
        public IHttpActionResult Edit(int id) {
            using (_dbContext) {
                var entity = _dbContext.Icons.FirstOrDefault(x => x.Id == id);
                var response = ResponseModelFactory.CreateInstance;
                response.SetData(_mapper.Map<Icon, IconCreateViewModel>(entity));
                return Ok(response);
            }
        }

        /// <summary>
        /// 保存编辑后的图标信息
        /// </summary>
        /// <param name="model">图标视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("icon/edit")]
        public IHttpActionResult Edit(IconCreateViewModel model) {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Code.Trim().Length <= 0) {
                response.SetFailed("请输入图标名称");
                return Ok(response);
            }
            using (_dbContext) {
                if (_dbContext.Icons.Count(x => x.Code == model.Code && x.Id != model.Id) > 0) {
                    response.SetFailed("图标已存在");
                    return Ok(response);
                }
                var entity = _dbContext.Icons.FirstOrDefault(x => x.Id == model.Id);
                entity.Code = model.Code;
                entity.Color = model.Color;
                entity.Custom = model.Custom;
                entity.Size = model.Size;
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
        /// 删除图标
        /// </summary>
        /// <param name="ids">图标ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        [Route("icon/delete/{ids}")]
        public IHttpActionResult Delete(string ids) {
            var response = ResponseModelFactory.CreateInstance;
            response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
            return Ok(response);
        }

        /// <summary>
        /// 恢复图标
        /// </summary>
        /// <param name="ids">图标ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        [Route("icon/recover/{ids}")]
        public IHttpActionResult Recover(string ids) {
            var response = ResponseModelFactory.CreateInstance;
         
            response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">图标ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        [Route("icon/batch")]
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
        /// 创建图标
        /// </summary>
        /// <param name="model">多行图标视图</param>
        /// <returns></returns>
        [HttpPost]
        [Route("icon/import")]
        public IHttpActionResult Import(IconImportViewModel model) {
            var response = ResponseModelFactory.CreateInstance;
          
            if (model.Icons.Trim().Length <= 0) {
                response.SetFailed("没有可用的图标");
                return Ok(response);
            }
            var models = model.Icons.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => new Icon
            {
                Code = x.Trim(),
                CreatedByUserId = AuthContextService.CurrentUser.UserId,
                CreatedOn = DateTime.Now,
                CreatedByUserName = "超级管理员"
            });
            using (_dbContext) {
                _dbContext.Icons.AddRange(models);
                _dbContext.SaveChanges();
                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除图标
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">图标ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids) {
            using (_dbContext) {
                var parameters = ids.Split(',').Select((id, index) => new SqlParameter($"p{index}", id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => $"@{p.ParameterName}"));
                var sql = string.Format("UPDATE Icon SET IsDeleted=@IsDeleted WHERE Id IN ({0})", parameterNames);
                parameters.Add(new SqlParameter("IsDeleted", (int)isDeleted));
                _dbContext.Database.ExecuteSqlCommand(sql, parameters.ToArray());
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        /// <summary>
        /// 删除图标
        /// </summary>
        /// <param name="status">图标状态</param>
        /// <param name="ids">图标ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateStatus(UserStatus status, string ids) {
            using (_dbContext) {
                var parameters = ids.Split(',').Select((id, index) => new SqlParameter($"p{index}", id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => $"@{p.ParameterName}"));
                var sql = string.Format("UPDATE Icon SET Status=@Status WHERE Id IN ({0})", parameterNames);
                parameters.Add(new SqlParameter("Status", (int)status));
                _dbContext.Database.ExecuteSqlCommand(sql, parameters.ToArray());
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
    }
}