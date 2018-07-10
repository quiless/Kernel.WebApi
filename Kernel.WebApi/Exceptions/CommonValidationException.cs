using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace Kernel.WebApi.Exceptions
{
    public class CommonValidationException : System.Exception
    {
        public List<ValidationResult> validationResults { get; set; }
        public CommonValidationException(string message)
        {
            if (validationResults == null)
                validationResults = new List<ValidationResult>();

            validationResults.Add(new ValidationResult(message, null, "", "", null));
        }
        public CommonValidationException() : base() { }

        public CommonValidationException(List<ValidationResult> results)
            : base()
        {
            validationResults = results;
        }

        public override string ToString()
        {
            String retorno = String.Empty;
            foreach (var validation in validationResults)
            {
                retorno += "\n " + (validation.Key != null ? validation.Key : "") + " -> " + (validation.Message != null ? validation.Message : "");
            }
            return retorno;
        }

        public IList<string> GetErrors()
        {
            List<String> retorno = new List<String>();
            foreach (var validation in validationResults)
            {
                retorno.Add(validation.Message);
            }
            return retorno;
        }
    }
}