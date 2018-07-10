using Kernel.WebApi.Entities;
using Kernel.WebApi.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using ChinhDo.Transactions;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Kernel.WebApi.Exceptions;
using System.Transactions;
using System.Data;

namespace Kernel.WebApi.BusinessRules
{
    public class CoreBusinessRules
    {
        #region Private Properties
        private CoreProvider Provider { get; set; }
        #endregion

        #region ctor
        public CoreBusinessRules()
        {
            Provider = new CoreProvider();
        }
        #endregion

        #region Authentication

        public AuthenticationResult Authenticate(string username, string password)
        {

            AuthenticationResult authResult = Provider.Authenticate(username, password);

            return authResult;
        }

        public AuthenticationResult AuthenticateToken(string token)
        {

            AuthenticationResult returnValue = new AuthenticationResult();
            returnValue.Result = ResultType.Failed;

            returnValue.Result = ResultType.Success;

            return returnValue;
        }


        #endregion

        #region User

        public UserInfo GetUserInfoPersonLogged(int PersonId)
        {
            return Provider.GetUserInfoPersonLogged(PersonId);
        }

        public bool SaveUserInfo (UserInfo entity)
        {
            this.Provider.SaveUserInfo(entity);
            return true;
        }

        #endregion
    }
}