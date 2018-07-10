using Kernel.WebApi.Entities;
using Kernel.WebApi.Upload;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Kernel.WebApi.Common
{
    public class ApplicationContext
    {

        public IDictionary<string, object> Session { get; set; }

      
        public const string PID = "PID";
        

        public ApplicationContext()
        {
            Session = new Dictionary<string, object>();
        }

        private static ApplicationContext _appContext;

        public static ApplicationContext Current
        {
            get
            {

                if (_appContext == null)
                {
                    _appContext = new ApplicationContext();
                }

                return _appContext;
            }
        }


        public void FillClaimsIdentity(ClaimsIdentity claimsIdentity, UserInfo userInfo)
        {
            claimsIdentity.AddClaim(new Claim(PID, userInfo.Id.ToString()));
            
        }

      

        public int GetPersonIdUserAuthenticated()
        {
            var result = this.Identity.FindFirst(PID)?.Value;
            return result == null ? 0 : Convert.ToInt32(result);
        }

     
        #region TokenInformation
        public ClaimsIdentity Identity
        {
            get
            {
                //.FindFirstValue(ClaimTypes.Name)  
                return (ClaimsIdentity)HttpContext.Current.User.Identity;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }
        #endregion


        #region Anexos Context
        public static class AttachmentContext
        {
            public static void ClearAttachmentsCache()
            {
                HttpContext.Current.Session.Remove("_AttachmentsUpload");
            }

            public static List<FlowFile> GetAttachmentsCacheByIdentifiers(IList<String> UniqueIdentifiers)
            {
                //Identifier
                if (UniqueIdentifiers == null || UniqueIdentifiers.Count == 0)
                    return null;
                var SessionAtachments = ApplicationContext.Current.Session;
                if (SessionAtachments != null)
                {
                    if (SessionAtachments.ContainsKey("_AttachmentsUpload"))
                    {
                        List<FlowFile> resultFiles = new List<FlowFile>();
                        var files = ((IEnumerable)SessionAtachments["_AttachmentsUpload"]).Cast<FlowFile>().ToList();

                        foreach (FlowFile file in files)
                        {
                            if (UniqueIdentifiers.Contains(file.Identifier))
                                resultFiles.Add(file);
                        }

                        return resultFiles;
                    }
                    return null;
                }
                else
                {
                    return null;
                }

            }

            public static void RemoveAttachmentsCacheByIdentifiers(List<String> UniqueIdentifiers)
            {
                if (HttpContext.Current.Session["_AttachmentsUpload"] != null && UniqueIdentifiers != null && UniqueIdentifiers.Count > 0)
                {
                    List<FlowFile> resultFiles = new List<FlowFile>();
                    var files = ((IEnumerable)HttpContext.Current.Session["_AttachmentsUpload"]).Cast<FlowFile>().ToList();

                    List<FlowFile> filesToRemove = new List<FlowFile>();
                    foreach (FlowFile file in files)
                    {
                        if (UniqueIdentifiers.Contains(file.Identifier))
                            filesToRemove.Add(file);
                    }

                    foreach (FlowFile file in filesToRemove)
                    {
                        files.Remove(file);
                    }

                    HttpContext.Current.Session["_AttachmentsUpload"] = files;

                }

            }

            public static List<FlowFile> AttachmentsCache
            {
                get
                {
                    var session = ApplicationContext.Current.Session;
                    if (session.ContainsKey("_AttachmentsUpload"))
                        return ((IEnumerable)session["_AttachmentsUpload"]).Cast<FlowFile>().ToList();
                    else
                        return null;

                }
                set
                {
                    var session = ApplicationContext.Current.Session;
                    if (session.ContainsKey("_AttachmentsUpload"))
                    {
                        if (session["_AttachmentsUpload"] != null)
                        {
                            session["_AttachmentsUpload"] = value;
                        }
                    }

                    else
                        session.Add("_AttachmentsUpload", value);

                }
            }
        }

        #endregion
    }
}