using Kernel.WebApi.Entities;
using Kernel.WebApi.Exceptions;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Kernel.WebApi.Entities
{
    public abstract class  EntityBase<T> 
    {
        public virtual bool IsNullOrEmptyOrEqualZero(object obj)
        {
            bool result = false;

            if (obj is int)
            {
                result = String.IsNullOrEmpty(Convert.ToString(obj)) || (int)obj == 0;
            }
            else if (obj is double)
            {
                result = String.IsNullOrEmpty(Convert.ToString(obj)) || (double)obj == 0;
            }
            else if (obj is decimal)
            {
                result = String.IsNullOrEmpty(Convert.ToString(obj)) || (decimal)obj == 0;
            }
            else if (obj is string)
            {
                result = String.IsNullOrEmpty((String)obj);
            }
            else if (obj is DateTime)
            {
                result = Convert.ToDateTime(obj) == DateTime.MinValue;
            }
            else
            {
                if (obj == null)
                    result = true;
            }
            return result;
        }


        public ValidationResult formatMessage(string tag, string key, string message)
        {
            return new ValidationResult(
                message: message,
                target: this,
                key: key,
                tag: tag,
                validator: null
            );
        }

        public ValidationResults GetErrors<TValidation>()
        {
            Validator<TValidation> validator = ValidationFactory.CreateValidator<TValidation>();
            return validator.Validate(this);
        }

        public string GetMessageErrors<TValidation>()
        {
            Validator<TValidation> validator = ValidationFactory.CreateValidator<TValidation>();
            ValidationResults results = validator.Validate(this);
            if (!results.IsValid)
            {
                StringBuilder builder = new StringBuilder();
                foreach (ValidationResult result in results)
                {
                    builder.AppendLine(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "{0}: {1}",
                            result.Tag,
                            result.Message));
                }

                return builder.ToString();
            }

            return string.Empty;
        }

        public virtual void ExecuteValidation()
        {
            ValidationResults validationResults = this.GetErrors<T>();

            if (!validationResults.IsValid)
                throw new CommonValidationException(validationResults.ToList<ValidationResult>());
        }

        public virtual T ToDerived<T>() where T : new()
        {
            T tDerived = new T();
            foreach (PropertyInfo propBase in this.GetType().GetProperties())
            {
                try
                {
                    PropertyInfo propDerived = typeof(T).GetProperty(propBase.Name);
                    if (propDerived.CanWrite)
                        propDerived.SetValue(tDerived, propBase.GetValue(this, null), null);
                }
                catch (Exception ex)
                {
                }
            }
            return tDerived;
        }

    }
        
}