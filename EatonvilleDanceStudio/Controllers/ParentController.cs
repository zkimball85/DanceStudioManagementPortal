using EatonvilleDanceStudio.Data;
using EatonvilleDanceStudio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EatonvilleDanceStudio.Controllers;

[Authorize] // Locks this down to only logged-in users
public class ParentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ParentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Dashboard()
    {
        // Get the currently logged-in user's Identity ID
        var userId = _userManager.GetUserId(User);
        
        // Find their linked ParentProfile AND bring along all their Dancers
        var parentProfile = await _context.ParentProfiles
            .Include(p => p.Dancers)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (parentProfile == null)
        {
            return NotFound("Parent profile not found. Please complete registration.");
        }

        return View(parentProfile);
    }

    public IActionResult AddDancer()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDancer(Dancer model)
    {
        var userId = _userManager.GetUserId(User);
        var parentProfile = await _context.ParentProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (parentProfile == null) return NotFound();

        // MVC will complain that ParentProfileId and Parent are missing from the HTML form. 
        // We strip them from the validation state because we are setting them manually right here!
        ModelState.Remove("ParentProfileId");
        ModelState.Remove("Parent");
        ModelState.Remove("ProfilePhotoPath"); // Handled later

        if (ModelState.IsValid)
        {
            model.ParentProfileId = parentProfile.Id;
            _context.Dancers.Add(model);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Dashboard));
        }

        return View(model);
    }
}