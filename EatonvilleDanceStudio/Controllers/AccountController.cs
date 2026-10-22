using System.Threading.Tasks;
using EatonvilleDanceStudio.Data;
using EatonvilleDanceStudio.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EatonvilleDanceStudio.Controllers
{
    // This controller manages user accounts. Right now,
    // it handles the GET and POST for the registration page to ensure
    // the ToS is strictly validated before saving the user to the database.
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // This endpoint displays the empty form when
        // a user navigates to /Account/Register.
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // This catches the form submission. If the ToS 
        // checkbox is empty, ModelState.IsValid will evaluate to false, 
        // and we immediately kick them back to the view with the red error.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we hit this line, validation failed (e.g., ToS unchecked or registration error).
            // Redisplay the form with their data so they can see the errors.
            return View(model);
        }
    }
}