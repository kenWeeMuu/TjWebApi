using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using WebApi.Extensions;

namespace WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {


            // Web API 配置和服务
            config.SetCorsPolicyProviderFactory(new CorsPolicyFactory());
            //  var cors = new EnableCorsAttribute("localhost:9000", "*", "*");
            config.EnableCors();


            config.Filters.Add(new AuthorizeAttribute());
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
