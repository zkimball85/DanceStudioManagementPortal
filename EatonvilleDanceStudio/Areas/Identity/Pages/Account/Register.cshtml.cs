// Areas/Identity/Pages/Account/Register.cshtml.cs
// Handles user registration and immediately provisions the linked Parent and Dancer profiles

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using EatonvilleDanceStudio.Data;
using EatonvilleDanceStudio.Models;

namespace EatonvilleDanceStudio.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;
    private readonly ApplicationDbContext _context;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = default!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
        
        [Required]
        [StringLength(50)]
        [Display(Name = "Parent First Name")]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Parent Last Name")]
        public required string LastName { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Primary Phone")]
        public required string PrimaryPhone { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Emergency Contact Name")]
        public required string EmergencyContactName { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Emergency Contact Phone")]
        public required string EmergencyContactPhone { get; set; }

        [Required]
        [Display(Name = "Dancer First Name")]
        public required string DancerFirstName { get; set; }

        [Required]
        [Display(Name = "Dancer Last Name")]
        public required string DancerLastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Dancer Date of Birth")]
        public DateTime? DancerDob { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        if (ModelState.IsValid)
        {
            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // Create and save the Parent Profile linked to the new Identity User
                var parentProfile = new ParentProfile
                {
                    UserId = user.Id,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    PrimaryPhone = Input.PrimaryPhone,
                    EmergencyContactName = Input.EmergencyContactName,
                    EmergencyContactPhone = Input.EmergencyContactPhone,
                    AccountStatus = "Pending",
                    User = null
                };

                _context.ParentProfiles.Add(parentProfile);
                await _context.SaveChangesAsync();

                // Create and save the initial Dancer linked to the Parent Profile
                var dancer = new Dancer
                {
                    ParentProfileId = parentProfile.Id,
                    FirstName = Input.DancerFirstName,
                    LastName = Input.DancerLastName,
                    DateOfBirth = Input.DancerDob
                };

                _context.Dancers.Add(dancer);
                await _context.SaveChangesAsync();

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId, code, returnUrl },
                    protocol: Request.Scheme)!;

                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("Login");
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
}