using System.Linq;
using System.Net;
using System.Web.Http;
using ErpDb.Entitys;
using WebApi.Manager;

namespace WebApi.Controllers
{
    public class TokenController : ApiController
    {
        // THis is naive endpoint for demo, it should use Basic authentication to provide token or POST request
        [AllowAnonymous]
        public string Get(string username, string password)
        {
            var db = new ErpDbContext();
            var user = db.Users.FirstOrDefault(x => x.LoginName == "administrator");
            if (user!=null)
            {
                return JwtManager.GenerateToken(user);
            }

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        public bool CheckUser(string username, string password)
        {
            // should check in the database
            return true;
        }
    }
}
