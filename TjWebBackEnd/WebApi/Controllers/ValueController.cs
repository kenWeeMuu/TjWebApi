using System.Web.Http;
using System.Web.Http.Controllers;
using WebApi.Filters;

namespace WebApi.Controllers
{
    //[JwtAuthentication]
    public class ValueController : ApiController
    {

        private readonly ILogger _logger;

        public ValueController(ILogger logger)
        {
            _logger = logger;
        }
        public string Get()
        {
            this._logger.Write("Inside the 'Get' method of the '{0}' controller.", GetType().Name);
            return "value";
        }

        // GET api/values/5
        public string Get(int id) {
            this._logger.Write("Inside the 'Get' method of the '{0}' controller.", GetType().Name);
            return "value2";
        }

    }
}
