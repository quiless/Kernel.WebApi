using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kernel.WebApi.Entities
{
    [HasSelfValidation]
    public class UserInfo : EntityBase<UserInfo>
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public int? Gender { get; set; }

        public string Name { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int PatientId { get; set; }
        public string RG { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime UpdateDate { get; set; }
        public Patient Patient { get; set; }
        public bool IsDeleted { get; set; }


        #region "Validation"

        [SelfValidation]
        public void Validate(ValidationResults errorResults)
        {
            string tag = "UserInfo";
            if (IsNullOrEmptyOrEqualZero(this.Name))
            {
                errorResults.AddResult(formatMessage(tag, "Name", " Necessário preencher o campo email"));
            }

            if (IsNullOrEmptyOrEqualZero(this.Email))
            {
                errorResults.AddResult(formatMessage(tag, "Email"," Necessário preencher o campo nome"));
            }


            if (IsNullOrEmptyOrEqualZero(this.Password))
            {
                errorResults.AddResult(formatMessage(tag, "Password", " Necessário preencher o campo senha"));
            }

            if (this.Password != this.ConfirmPassword)
            {
                errorResults.AddResult(formatMessage(tag, "ConfirmPassword", " As senhas não conferem"));
            }

            if (IsNullOrEmptyOrEqualZero(this.PhoneNumber))
            {
                errorResults.AddResult(formatMessage(tag, "PhoneNumber", " Necessário preencher o campo celular"));
            }


        }


        #endregion
    }





}