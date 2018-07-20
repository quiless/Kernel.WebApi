using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Web.Http.ModelBinding;
using System.Web.Http.Controllers;
using System.Web.Http.ValueProviders;
using System.Globalization;

namespace Kernel.WebApi
{
    public class DecimalModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;

            if (valueResult.AttemptedValue.Contains(","))
            {
                throw new Exception("Some exception");
            }
            actualValue = Convert.ToDecimal(valueResult.AttemptedValue,
                CultureInfo.CurrentCulture);


            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            bindingContext.Model = actualValue;
            return true;
        }
    }

    public static class WebApiConfig
    {


        public static void Register(HttpConfiguration config)
        {


            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
              name: "DefaultApi",
              routeTemplate: "api/{controller}/{action}/{id}",
              defaults: new { id = RouteParameter.Optional }
            );



            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            config.BindParameter(typeof(decimal), new DecimalModelBinder());



       
        }
    }
}
