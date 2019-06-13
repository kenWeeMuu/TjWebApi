using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace WebApi.Extensions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class MyCorsPolicyAttribute : Attribute, ICorsPolicyProvider
    {
        private CorsPolicy _policy;

        public MyCorsPolicyAttribute() {
        
            _policy = new CorsPolicy
            {
                AllowAnyOrigin = true,
                AllowAnyMethod = true,
                AllowAnyHeader = true
            };

            // Add allowed origins.
            _policy.Origins.Add("http://localhost:9000");
            //   _policy.Origins.Add("http://www.contoso.com");
        }



        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return Task.FromResult(_policy);
        }
    }


    public class CorsPolicyFactory : ICorsPolicyProviderFactory
    {
        private readonly ICorsPolicyProvider _provider;

        public CorsPolicyFactory(MyCorsPolicyAttribute provider)
        {
            this._provider = provider;
        }

        public ICorsPolicyProvider GetCorsPolicyProvider(HttpRequestMessage request) {
            return _provider;
        }
    }
}