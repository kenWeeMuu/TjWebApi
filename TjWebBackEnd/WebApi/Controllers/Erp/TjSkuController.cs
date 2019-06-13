using System.Linq;
using System.Web.Http;
using AutoMapper;
using ErpDb.Entitys;
using WebApi.Extensions;

namespace WebApi.Controllers.Erp
{
    [RoutePrefix("api/v1/erp")]
    public class TjSkuController : ApiController
    {
        private readonly ErpDbContext _dbContext;
        private readonly IMapper _mapper;

        public TjSkuController(ErpDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("tjsku/create")]
        public IHttpActionResult Create(TjSku model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.SkuCode.Trim().Length <= 0)
            {
                response.SetFailed("请输入物料编码");
                return Ok(response);
            }

            using (_dbContext)
            {
                if (_dbContext.TjSkus.Any(x => x.SkuCode == model.SkuCode))
                {
                    response.SetFailed($"{model.SkuType}的物料编码{model.SkuCode}已经存在！");
                    return Ok(response);
                }

                var entity = new TjSku
                {
                    SkuCode = model.SkuCode,
                    SkuType = model.SkuType,
                    CName = model.CName,
                    EName = model.EName
                };
                _dbContext.TjSkus.Add(entity);
                _dbContext.SaveChanges();

                response.SetSuccess();
                return Ok(response);
            }
        }

        [HttpPost]
        [Route("tjsku/skus")]
        public IHttpActionResult Skus(RequestPayload.RequestPayload payload)
        {
            using (_dbContext)
            {
                var query = _dbContext.TjSkus.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x =>
                        x.SkuType.Contains(payload.Kw.Trim()) || x.SkuCode.Contains(payload.Kw.Trim()));
                }

                if (payload.FirstSort != null)
                {
                    query = query.OrderBy(payload.FirstSort.Field, payload.FirstSort.Direct == "DESC");
                }

                var list = query.Paged(payload.CurrentPage, payload.PageSize).ToList();
                var totalCount = query.Count();
                //   var data = list.Select(_mapper.Map<DncUser, UserJsonModel>);
                var response = ResponseModelFactory.CreateResultInstance;

                response.SetData(list, totalCount);
                return Ok(response);
            }
        }
    }
}