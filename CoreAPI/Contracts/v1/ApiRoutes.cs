using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Contracts.v1 
{
    public class ApiRoutes
    {
        public const string Version = "v1";
        public const string Root = "api";
        public const string Base = Root+"/"+Version +"/";
        public static class Products
        {
            public const string GetALL = Base + "products";
            public const string Create = "api/v1/products";
            public const string Get = "api/v1/products/{productid}";
        }
    }
}
