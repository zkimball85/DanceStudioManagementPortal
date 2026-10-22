using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EatonvilleDanceStudio.Attributes
{
    // Custom validation attribute to enforce a 
    // boolean is true, complete with client-side jQuery wiring.
    public class MustBeTrueAttribute : ValidationAttribute, 
        IClientModelValidator
    {
        public override bool IsValid(object value)
        {
            // Checks if the value is a boolean and if it is true.
            return value is bool booleanValue && booleanValue;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            // Injects the custom data-val tags into the HTML output
            // so jQuery unobtrusive validation can pick it up.
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-mustbetrue", ErrorMessage);
        }
    }
}