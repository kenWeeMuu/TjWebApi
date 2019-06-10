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



            new ApiConfig(config)
                .SetCors()
                .UseAutoMapper()
                .UseAutoFac()
                .SetJson()
                .ConfigureRoutes()
                .ConfigureExceptionHandling()
                .AddJwtAuth();
      


            // Web API 配置和服务

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
