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
            public const string Get = Base + "products/{productID}";
            public const string GetALL = Base + "products";
            public const string Create = Base + "products";
            public const string Update = Base + "products/{productID}";
            public const string Delete = Base + "products/{productID}";
          
        }

        public static class Identity
        {
            public const string Login = Base + "identity/Login";
            public const string SignUp = Base + "identity/Signup";
        }
    }
}
