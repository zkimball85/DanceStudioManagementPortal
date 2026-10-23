using System.ComponentModel.DataAnnotations;
using EatonvilleDanceStudio.Attributes;

namespace EatonvilleDanceStudio.ViewModels
{
    /// <summary>
    /// Represents the data model used for the user registration process,
    /// validating user inputs such as email, password, and agreement to terms of service.
    /// </summary>
    
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be " +
                                          "at least {2} characters long.", 
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password " +
                                            "and confirmation do not match.")]
        public string ConfirmPassword { get; set; }

        // The Range attribute strictly forces the boolean 
        // to be "true" instead of accepting "false".
        // We replaced Range with our custom attribute to fix the 
        // jQuery client-side validation casing bug.
        [Display(Name = "I agree to the Terms of Service")]
        [MustBeTrue(ErrorMessage = "You must agree to the Terms of " +
                                   "Service to register.")]
        public bool HasAgreedToTerms { get; set; }
    }
}