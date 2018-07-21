using Kernel.WebApi.BusinessRules;
using Kernel.WebApi.Common;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Kernel.WebApi.OAuthProvivers
{
    public class FastAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public CoreBusinessRules CoreBusinessRules { get; set; }

        public FastAuthorizationServerProvider()
        {
            CoreBusinessRules = new CoreBusinessRules();

        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var authResult = this.CoreBusinessRules.Authenticate(context.UserName, context.Password);

            if (authResult == null || authResult.Result != Entities.ResultType.Success)
            {
                context.SetError("Usuário e/ou senha são inválidos!");
                return;
            }


            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            ApplicationContext.Current.FillClaimsIdentity(identity, authResult.userInfo);
            context.Validated(identity);

        }
    }
}