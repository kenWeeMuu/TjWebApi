using System.Web.Http;
using System.Web.Http.Controllers;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [JwtAuthentication]
    public class ValueController : ApiController
    {
     
        public string Get()
        {
            return "value";
        }

        // GET api/values/5
        public string Get(int id) {

            return "value2";
        }

    }
}
