using ClosedXML.Excel;
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

namespace Kernel.WebApi.Controllers
{
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
        public IHttpActionResult SaveUserInfo(Entities.UserInfo entity)
        {
            return ApiResult<bool>(() =>
            {
                return this.CoreBusinessRules.SaveUserInfo(entity);
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
    }
}