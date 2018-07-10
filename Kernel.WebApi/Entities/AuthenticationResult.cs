using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kernel.WebApi.Entities
{
    public class AuthenticationResult
    {
        public UserInfo userInfo { get; set; }

        public ResultType Result { get; set; }

    }

    public enum ResultType
    {
        None,
        Success,
        Failed,
        InvalidLoginOrPassword
    }
}