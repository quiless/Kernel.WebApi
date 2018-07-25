using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kernel.WebApi.Entities
{
    [HasSelfValidation]
    public class MedicalResult : EntityBase<MedicalResult>   
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ResultDate { get; set; }
        public int RepeatDays { get; set; }
        public decimal MediumGlycogen { get; set; }
        public decimal PercentGlycogen { get; set; }
        public bool IsDeleted { get; set; }

        //Utilizado apenas na controller para enviar e-mail
        public bool SendEmailSMS { get; set; }
        public Patient Patient { get; set; }

        public string Uid { get; set; }
        #region "Validation"

        [SelfValidation]
        public void Validate(ValidationResults errorResults)
        {
            string tag = "Patient";
            if (IsNullOrEmptyOrEqualZero(this.PatientId))
            {
                errorResults.AddResult(formatMessage(tag, "PatientId", " Necessário informar o paciente"));
            }

            if (IsNullOrEmptyOrEqualZero(this.ResultDate))
            {
                errorResults.AddResult(formatMessage(tag, "ResultDate", "Informe a data do exame"));
            }


            if (IsNullOrEmptyOrEqualZero(this.PercentGlycogen))
            {
                errorResults.AddResult(formatMessage(tag, "PercentGlycogen", "Informe o resultado"));
            }
        }


        #endregion

    }
}