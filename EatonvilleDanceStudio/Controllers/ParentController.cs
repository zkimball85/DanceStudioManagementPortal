using EatonvilleDanceStudio.Data;
using EatonvilleDanceStudio.Models;
using EatonvilleDanceStudio.ViewModels;
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
        return View(new AddDancerViewModel() { FirstName = "", LastName = "" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDancer(AddDancerViewModel input)
    {
        var userId = _userManager.GetUserId(User);
        var parentProfile = await _context.ParentProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (parentProfile == null) return NotFound();

        if (ModelState.IsValid)
        {
            // We manually map the safe fields from the Input Model to the Database Model
            var newDancer = new Dancer
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                DateOfBirth = input.DateOfBirth,
                SkillLevel = input.SkillLevel,

                // These are set strictly by the server, totally immune to user forging
                ParentProfileId = parentProfile.Id,
                PhotoApprovalStatus = "None",
                ProfilePhotoPath = null
            };
            
            _context.Dancers.Add(newDancer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        return View(input);
    }
}