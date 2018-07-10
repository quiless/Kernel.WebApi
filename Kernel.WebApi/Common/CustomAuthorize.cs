using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Kernel.WebApi.Common
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        private string[] UserProfilesRequired { get; set; }

        public CustomAuthorize(params string[] userProfilesRequired)
        {
            //if (userProfilesRequired.Any(p => p.GetType().BaseType != typeof(Enum)))
            //    throw new ArgumentException("userProfilesRequired");

            this.UserProfilesRequired = userProfilesRequired.ToArray();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            
                var response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.MethodNotAllowed);
                response.Content = new System.Net.Http.StringContent("Você não tem permissão para fazer essa requisição.");
                actionContext.Response = response; //actionContext.Request.c(HttpStatusCode.Unauthorized, "Not allowed to access...bla bla");

        }
    }
}