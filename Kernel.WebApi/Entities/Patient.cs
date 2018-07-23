using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kernel.WebApi.Entities
{
    [HasSelfValidation]
    public class Patient : EntityBase<Patient>
    {
        public int Id { get; set; }         
        public string Email { get; set; }
        public int? Gender { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Name { get; set; }
        public string RG { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsDeleted { get; set; }


        #region "Validation"

        [SelfValidation]
        public void Validate(ValidationResults errorResults)
        {
            string tag = "Patient";
            if (IsNullOrEmptyOrEqualZero(this.Name))
            {
                errorResults.AddResult(formatMessage(tag, "Name", " Necessário preencher o campo email"));
            }

            if (IsNullOrEmptyOrEqualZero(this.Email))
            {
                errorResults.AddResult(formatMessage(tag, "Email"," Necessário preencher o campo nome"));
            }


            if (IsNullOrEmptyOrEqualZero(this.PhoneNumber))
            {
                errorResults.AddResult(formatMessage(tag, "PhoneNumber", " Necessário preencher o campo celular"));
            }


        }


        #endregion
    }





}