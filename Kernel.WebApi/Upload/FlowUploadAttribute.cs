using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Kernel.WebApi.Upload
{
    public class FlowUploadAttribute : ActionFilterAttribute
    {
        public FlowUploadAttribute(params string[] extensions)
        {
            Extensions = extensions;
            Size = 50000000;
        }
        public int Size { get; set; }
        public string[] Extensions { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var flowJs = new FlowJsRepo();
            var request = actionContext.Request;
            var validationRules = new FlowValidationRules();
            validationRules.AcceptedExtensions.AddRange(Extensions);
            validationRules.MaxFileSize = Size;
            var status = flowJs.PostChunk(HttpContext.Current.Request, Path.GetTempPath(), validationRules);

            if (status.Status == PostChunkStatus.Done)
            {

                var filepath = Path.Combine(Path.GetTempPath(), status.FileName);

                var p = actionContext.ActionDescriptor.GetParameters()
                    .FirstOrDefault(x => x.ParameterType == typeof(FlowFile));

                if (filepath != null)
                {
                    actionContext.ActionArguments[p.ParameterName] = new FlowFile
                    {
                        originalFilename = status.OriginalFileName,
                        flowFilename = status.FileName,
                        path = filepath,
                        Identifier = status.Identifier
                    };
                    return;
                }
            }


            actionContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Accepted);

            base.OnActionExecuting(actionContext);
        }
    }
}