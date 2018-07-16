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

        public bool SaveUserInfo(UserInfo entity)
        {

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {

                entity.ExecuteValidation();

                Patient Patient = new Patient();

                Patient.Email = entity.Email;
                Patient.Name = entity.Name;
                Patient.PhoneNumber = entity.PhoneNumber;
                Patient.RG = entity.RG;
                entity.PatientId = this.SavePatient(Patient);

                this.Provider.SaveUserInfo(entity);
                
                scope.Complete();
            }

            return true;
        }

        public bool VerifyPatientByEmail(string email)
        {
            return this.Provider.VerifyPatientByEmail(email);
        }


        public bool VerifyPatientByRG(string RG)
        {
            return this.Provider.VerifyPatientByRG(RG);
        }

        public int SavePatient(Patient entity)
        {
            var HasPatientSameRG = this.VerifyPatientByRG(entity.RG);
            var HasPatientSameEmail = this.VerifyPatientByEmail(entity.Email);

            if (HasPatientSameRG)
            {
                throw new CommonValidationException("O RG informado já possuí cadastro");
            }

            if (HasPatientSameEmail)
            {
                throw new CommonValidationException("O Email informado já possuí cadastro");
            }

            return this.Provider.SavePatient(entity);

        }

        public Patient GetPatientByRG(string RG)
        {
            return this.Provider.GetPatientByRG(RG);
        }

        #endregion
    }
}