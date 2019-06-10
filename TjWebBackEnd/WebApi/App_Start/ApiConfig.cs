using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using ErpDb.Entitys;
using ErpDb.Entitys.Auth;
using Newtonsoft.Json.Serialization;
using WebApi.Controllers;
using WebApi.Controllers.Auth;
using WebApi.Extensions;
using WebApi.Filters;
using WebApi.ViewModel;

namespace WebApi
{
    public class ApiConfig
    {
        private readonly HttpConfiguration _config;

        public ApiConfig(HttpConfiguration config)
        {
            _config = config;
        }


        public ApiConfig SetJson()
        {
            //JSON 配置
            //            var formatters = GlobalConfiguration.Configuration.Formatters;
            //            var jsonFormatter = formatters.JsonFormatter;
            //            var settings = jsonFormatter.SerializerSettings;
            //            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            _config.Formatters.Clear();
            _config.Formatters.Add(new JsonMediaTypeFormatter());

            _config.Formatters.JsonFormatter.SerializerSettings.ContractResolver
                = new CamelCasePropertyNamesContractResolver();

            _config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
            _config.Formatters.JsonFormatter.SupportedMediaTypes.Clear();

            // Delete the line below if you nee more content type. E.g. application/text
            _config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));

            return this;
        }

        public ApiConfig SetCors()
        {
            _config.SetCorsPolicyProviderFactory(new CorsPolicyFactory());
            //  var cors = new EnableCorsAttribute("localhost:9000", "*", "*");
            _config.EnableCors();

            return this;
        }

        /// <summary>
        /// Configures Web API routes.
        /// </summary>
        public ApiConfig ConfigureRoutes()
        {
            _config.MapHttpAttributeRoutes();

            return this;
        }

        public ApiConfig AddJwtAuth()
        {
            _config.Filters.Add(new AuthorizeAttribute());

            return this;
        }

        public ApiConfig ConfigureExceptionHandling()
        {
            _config.Services.Replace(typeof(IExceptionHandler), new ApiExceptionHandler());
            _config.Services.Add(typeof(IExceptionLogger), new ApiExceptionLogger(new Logger()));

            return this;
        }


        public ApiConfig UseAutoFac()
        {
            var builder = new ContainerBuilder();

            // Register Web API controller in executing assembly.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL - Register the filter provider if you have custom filters that need DI.
            // Also hook the filters up to controllers.
            builder.RegisterWebApiFilterProvider(_config);
            builder.RegisterType<JwtAuthenticationAttribute>()
                .AsWebApiAuthenticationFilterFor<ApiController>().InstancePerRequest();
            // Register a logger service to be used by the controller and middleware.
            builder.Register(c => new Logger()).As<ILogger>().InstancePerRequest();
            builder.Register(c => new ErpDbContext()).InstancePerRequest();
            builder.Register(r => mapper).As<IMapper>().InstancePerRequest();
            // Autofac will add middleware to IAppBuilder in the order registered.
            // The middleware will execute in the order added to IAppBuilder.
//            builder.RegisterType<FirstMiddleware>().InstancePerRequest();
//            builder.RegisterType<SecondMiddleware>().InstancePerRequest();

            // Create and assign a dependency resolver for Web API to use.
            var container = builder.Build();
            _config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            return this;
        }

        private IMapper mapper { get; set; }

        public ApiConfig UseAutoMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MenuCreateViewModel, Menu>();
                cfg.CreateMap<MenuEditViewModel, Menu>();

                cfg.CreateMap<User, UserJsonModel>();
                 cfg.CreateMap<UserCreateViewModel, User>();
                 cfg.CreateMap<User, UserEditViewModel>();
                 cfg.CreateMap<Role, RoleJsonModel>();
                 cfg.CreateMap<RoleCreateViewModel, Role>();
                 cfg.CreateMap<Role, RoleCreateViewModel>();

  
               //  cfg.CreateMap<Role, RoleCreateViewModel>();
            });


            // only during development, validate your mappings; remove it before release
            // configuration.AssertConfigurationIsValid();
            // use DI (http://docs.automapper.org/en/latest/Dependency-injection.html) or create the mapper yourself
            mapper = configuration.CreateMapper();

            return this;
        }
    }
}