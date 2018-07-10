using Kernel.WebApi.Common;
using Kernel.WebApi.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Kernel.WebApi.Controllers
{
    public class AttachmentController : ApiControllerBase
    {
        [HttpGet]
        [Route("UploadFile")]
        public IHttpActionResult UploadFile()
        {
            //HttpResponseMessage
            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }

        [HttpPost]
        [Route("UploadFile")]
        [FlowUpload()]
        //[ActionName("UploadImage")]
        public IHttpActionResult UploadFile([FromUri] FlowFile file)
        {
            //if (ClaimArchiveFlowFile != null)
            //    System.IO.File.Delete(ClaimArchiveFlowFile.path);
            // return Ok();
            AddAttachment(file);
            return Json(new { derp = file.flowFilename });
        }

        [HttpPost]
        public IHttpActionResult ClearSessionAttachments()
        {
            return ApiResult<bool>(() =>
            {
                ClearAttachments();
                return true;
            });

        }

        public void AddAttachment(FlowFile file)
        {

            List<FlowFile> lst = ApplicationContext.AttachmentContext.AttachmentsCache;
            if (lst == null)
            {
                lst = new List<FlowFile>();
                lst.Add(file);
            }
            else
            {
                lst.Add(file);
            }
            ApplicationContext.AttachmentContext.AttachmentsCache = lst;
        }

        public void ClearAttachments()
        {
            ApplicationContext.AttachmentContext.ClearAttachmentsCache();
        }
    }
}