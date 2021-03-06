﻿using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Kernel.WebApi.BusinessRules;
using Kernel.WebApi.Common;
using Kernel.WebApi.Entities;
using Kernel.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Mail;
using System.Configuration;
using OpenHtmlToPdf;

namespace Kernel.WebApi.Controllers
{
    [Authorize]
    public class CoreController : ApiControllerBase
    {
        #region Constructor

        public CoreBusinessRules CoreBusinessRules { get; set; }

        public CoreController()
        {
            CoreBusinessRules = new CoreBusinessRules();

        }

        #endregion

        #region UserInfo



        [HttpPost]
        public IHttpActionResult GetUserInfoPersonLogged()
        {
            return ApiResult<Entities.UserInfo>(() =>
            {
                return this.CoreBusinessRules.GetUserInfoPersonLogged(ApplicationContext.Current.GetPersonIdUserAuthenticated());
            });
        }

        [HttpPost]
        public IHttpActionResult SavePatient(Patient entity)
        {
            return ApiResult<int>(() =>
            {
               return this.CoreBusinessRules.SavePatient(entity);
             
            });
        }


        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult SaveUserInfo(Entities.UserInfo entity)
        {
            return ApiResult<bool>(() =>
            {
                return this.CoreBusinessRules.SaveUserInfo(entity);
            });
        }

        [HttpPost]
        public IHttpActionResult GetPatientByRG([FromBody] string RG)
        {
            return ApiResult<Patient>(() =>
            {

                return this.CoreBusinessRules.GetPatientByRG(RG);
            });
        }


        #endregion

        #region ClassVideoStream

        public class VideoStream
        {
            private readonly string _filename;

            public VideoStream(string filename)
            {
                _filename = filename;
            }

            public async Task WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
            {
                try
                {
                    var buffer = new byte[65536];

                    using (var video = File.Open(_filename, FileMode.Open, FileAccess.Read))
                    {
                        var length = (int)video.Length;
                        var bytesRead = 1;

                        while (length > 0 && bytesRead > 0)
                        {
                            bytesRead = video.Read(buffer, 0, Math.Min(length, buffer.Length));
                            await outputStream.WriteAsync(buffer, 0, bytesRead);
                            length -= bytesRead;
                        }
                    }
                }
                catch (HttpException ex)
                {
                    return;
                }
                finally
                {
                    outputStream.Close();
                }
            }
        }

        #endregion

        #region MedicalResult


        [HttpPost]
        public IHttpActionResult SaveMedicalResult(MedicalResult entity)
        {
            return ApiResult<MedicalResult>(() =>
            {
                var PersonIdRequester = ApplicationContext.Current.GetPersonIdUserAuthenticated();
                return this.CoreBusinessRules.SaveMedicalResult(entity, PersonIdRequester);
            });
        }

        [HttpPost]
        public IHttpActionResult SendMedicalResultSMSEmail(MedicalResult entity)
        {
            return ApiResult<bool>(() =>
            {
                var PersonIdRequester = ApplicationContext.Current.GetPersonIdUserAuthenticated();
                return this.CoreBusinessRules.SendMedicalResultSMSEmail(entity, PersonIdRequester);
            });
        }

        [HttpPost]
        public IHttpActionResult GetMedicalResults()
        {
            return ApiResult<IList<MedicalResult>>(() =>
            {
                var PersonIdRequester = ApplicationContext.Current.GetPersonIdUserAuthenticated();
                var result = this.CoreBusinessRules.GetMedicalResults(PersonIdRequester);
                return result;
            });
        }
        #endregion

        #region Patient 

        [HttpPost]
        public IHttpActionResult ImportMedicalResults([FromBody] string RG)
        {
            return ApiResult<bool>(() =>
            {
                var PersonIdRequester = ApplicationContext.Current.GetPersonIdUserAuthenticated();

                return this.CoreBusinessRules.ImportMedicalResults(PersonIdRequester, RG);
            });
        }

        [HttpPost]
        public IHttpActionResult SaveTextConfig(IList<UserInfoTextConfig> lstTextConfigs)
        {
            return ApiResult<bool>(() =>
            {
                var PersonIdRequester = ApplicationContext.Current.GetPersonIdUserAuthenticated();


                return CoreBusinessRules.SaveTextConfig(lstTextConfigs, PersonIdRequester);
            });
        }

        [HttpPost]
        public IHttpActionResult ResetTextConfig()
        {
            return ApiResult<bool>(() =>
            {
                var PersonIdRequester = ApplicationContext.Current.GetPersonIdUserAuthenticated();

                return CoreBusinessRules.ResetTextConfig(PersonIdRequester);
            });
        }

        [HttpPost]
        public IHttpActionResult GetUserTextConfig()
        {
            return ApiResult<IList<UserInfoTextConfig>>(() =>
            {
                var PersonIdRequester = ApplicationContext.Current.GetPersonIdUserAuthenticated();
                return CoreBusinessRules.GetUserTextConfig(PersonIdRequester);
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DownloadFile([FromUri] Guid uid)
        {
            var path = ConfigurationManager.AppSettings["pdfs"] + @"\" + uid + ".pdf";
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open,FileAccess.Read);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            result.Content.Headers.ContentDisposition.FileName = Guid.NewGuid().ToString() + Path.GetExtension(path);

            var contentType = System.Web.MimeMapping.GetMimeMapping(System.IO.Path.GetFileName(path));
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            result.Content.Headers.ContentLength = stream.Length;
            return result;


            //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            //result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType =
            //    new MediaTypeHeaderValue("application/octet-stream");
            //return result;

            //         
            //var video = new VideoStream(path);
            //var response = Request.CreateResponse();
            //var contentType = System.Web.MimeMapping.GetMimeMapping(System.IO.Path.GetFileName(path + @"\" + uid + ".pdf"));
            //response.Content = new PushStreamContent(video.WriteToStream, new MediaTypeHeaderValue(contentType));
            //return response;
        }

        #endregion

        #region PDF
        [HttpPost]
        public IHttpActionResult GetPdfData([FromBody] int MedicalResultId)
        {
            return ApiResult<string>(() =>
            {
               
                var PersonIdRequester = ApplicationContext.Current.GetPersonIdUserAuthenticated();
                var html = this.CoreBusinessRules.GetPdfData(PersonIdRequester, MedicalResultId);

                string pdfdir = ConfigurationManager.AppSettings["pdfs"];
                var pdfPath = pdfdir + @"\" + Guid.NewGuid().ToString() + ".pdf";
                if (!File.Exists(pdfPath))
                {
                    if (!Directory.Exists(pdfdir))
                        Directory.CreateDirectory(pdfdir);

                    var pdf = Pdf.From(html)
                                .OfSize(PaperSize.A4)
                                .WithTitle("AC1 NOW - RESUMO DO EXAME")
                                .Content();


                    File.WriteAllBytes(pdfPath, pdf);
                }

                return html;
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult TestMethod()
        {
            return ApiResult<string>(() =>
            {
                return "ok";
            });
        }
        #endregion
    }
}