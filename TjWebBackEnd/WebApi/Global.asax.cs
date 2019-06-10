using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Newtonsoft.Json.Serialization;

namespace WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start() {       

         
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //JSON 配置
            var formatters = GlobalConfiguration.Configuration.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
