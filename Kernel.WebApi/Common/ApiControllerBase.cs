using Kernel.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Kernel.WebApi.Common
{
    public class ApiControllerBase : ApiController
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
        }
     

        protected virtual IHttpActionResult ApiResult<T>(Func<T> fn)
        {
            try
            {

                var response = fn();

                return new ApiResponseResult<T>(response);
            }
            catch (CommonValidationException valEx)
            {
                var errors = (valEx.GetErrors());
                return new ApiResponseResult<IList<String>>(errors, HttpStatusCode.BadRequest);
            }

            catch (Exception ex)
            {
                var errors = new List<String>(new string[] { ex.Message });
                return new ApiResponseResult<IList<String>>(errors, HttpStatusCode.BadRequest);
            }
        }
    }
}