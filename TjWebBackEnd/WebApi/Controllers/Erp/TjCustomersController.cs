using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ErpDb.Entitys;
using WebApi.Extensions;
using WebApi.Extensions.AuthContext;
using WebApi.RequestPayload.Rbac;

namespace WebApi.Controllers.Erp
{
    [RoutePrefix("api/v1/erp")]
    public class TjCustomerController : ApiController
    {
        private   ErpDbContext _dbContext;
        private   IMapper mapper;

 

        public TjCustomerController(ErpDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            this.mapper = mapper;
        }
        [Route("customer/list")]
        [HttpPost ]
        public IHttpActionResult List(TjRequestPayload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            if (string.IsNullOrEmpty(payload.Guid))
            {
                response.SetError("你想干什么?");
                return Ok(response);
            }

            var query = _dbContext.TjCustomers.AsNoTracking().AsQueryable();
            //            if (payload.FirstSort != null) {
            //                query = query.OrderBy(payload.FirstSort.Field, payload.FirstSort.Direct == "DESC");
            //            }


            if (AuthContextService.IsSupperAdministator)
            {
                if (payload.Kw != "")
                {
                    var list = query.Contains(payload.FirstSort.Field, payload.Kw)
                        //TODO 可以用这里来测试全局错误日志
                        .OrderByDescending(x => x.Id)
                        .Paged(payload.CurrentPage, payload.PageSize);
                    var totalCount = list.Count();
                    response.SetData(list, totalCount);
                }
                else
                {
                    //  var list = query.OrderBy(x => x.Id).Paged(payload.CurrentPage, payload.PageSize) ;
                    var list = query.OrderByDescending(x => x.Id).Paged(payload.CurrentPage, payload.PageSize);
                    var totalCount = query.Count();
                    response.SetData(list, totalCount);
                }
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("tjcustomer/create")]
        public IHttpActionResult Create(TjCustomer customer)
        {
            var response = ResponseModelFactory.CreateInstance;

            using (_dbContext)
            {
                customer.Owner =AuthContextService.CurrentUser.UserId;
                _dbContext.TjCustomers.Add(customer);
                _dbContext.SaveChanges();
            }

            response.SetSuccess();
            return Ok(response);
        }


        [HttpGet]
        [Route("tjcustomer/edit/{id}")]
        public IHttpActionResult Edit(int id)
        {
            using (_dbContext)
            {
                var entity = _dbContext.TjCustomers.FirstOrDefault(x => x.Id == id);
                var response = ResponseModelFactory.CreateInstance;
                response.SetData(entity);
                return Ok(response);
            }
        }



        [HttpPost]
      [Route("tjcustomer/edit")]
        public IHttpActionResult Edit(TjCustomer model)
        {
            var response = ResponseModelFactory.CreateInstance;

            using (_dbContext)
            {
                var entity = _dbContext.TjCustomers.FirstOrDefault(x => x.Id == model.Id);
                if (entity == null)
                {
                    response.SetFailed("该客户不存在");
                    return Ok(response);
                }

                entity.Name = model.Name;
                entity.Phone = model.Phone;
                entity.Email = model.Email;
                entity.Company = model.Company;
                entity.Region = model.Region;
                entity.Location = model.Location;
                entity.Industry = model.Industry;
                entity.Owner = AuthContextService.CurrentUser.UserId;
                _dbContext.SaveChanges();
                response = ResponseModelFactory.CreateInstance;
                return Ok(response);
            }
        }
    }
}