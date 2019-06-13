using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using ErpDb.Entitys;
using ErpDb.Entitys.Auth;
using WebApi.Manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApi.Controllers
{
    [RoutePrefix("api/token")]
    public class TokenController : ApiController
    {
        // THis is naive endpoint for demo, it should use Basic authentication to provide token or POST request
        [AllowAnonymous]
        [Route("nimabi/c")]
        [HttpGet]
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
        [HttpGet]
        [AllowAnonymous]
        [Route("ccc")]
        public string bc()
        {
            string json = File.ReadAllText(@"c:\Users\Administrator\Documents\customer.txt",Encoding.UTF8);
           var list = JsonConvert.DeserializeObject<List<JObject>>(json);

         var customers =  from jo in list
               select new TjCustomer
               {
                   Name = jo["name"].ToString(),
                   Email = jo["email"].ToString(),
                   Phone = jo["phone"].ToString(),
                   Company =jo["company"].ToString(),
                   Region =jo["region"].ToString(),
                   Location = jo["location"].ToString(),
                   Industry = jo["industry"].ToString(),
                   Owner =0,
                   ServiceBy = 0
               };
         var db = new ErpDbContext();
         db.TjCustomers.AddRange(customers);
         db.SaveChanges();


           return "ok";
        }
        public bool CheckUser(string username, string password)
        {
            // should check in the database
            return true;
        }
    }
}
