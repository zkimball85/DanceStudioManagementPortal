using EatonvilleDanceStudio.Data;
using EatonvilleDanceStudio.Models;
using EatonvilleDanceStudio.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EatonvilleDanceStudio.Controllers;

/// <summary>
/// ParentController handles interactions related to parental user functionality within the application.
/// This includes retrieving and managing parent profiles, associated dancers, and their enrollments.
/// </summary>

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
    
    // GET: /Parent/Dashboard
    /// <summary>
    /// Displays the parent dashboard, providing an overview of the parent's profile,
    /// dancers, enrollments, announcements, events, and competition placements.
    /// </summary>
    /// <returns>An asynchronous result that renders the parent dashboard view.</returns>
    
    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);
        
        // Use a Filtered Include to only pull active enrollments from the database
        var parentProfile = await _context.ParentProfiles
            .Include(p => p.Dancers)
            .ThenInclude(d => d.Enrollments.Where(e => e.Status == "Enrolled" || e.Status == "Waitlisted"))
            .ThenInclude(e => e.DanceClass)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (parentProfile == null) return NotFound("Parent profile not found.");

        // Flatten the already-filtered enrollments into one list for the UI
        var familyEnrollments = parentProfile.Dancers
            .SelectMany(d => d.Enrollments)
            .ToList();

        var viewModel = new ParentDashboardViewModel
        {
            Profile = parentProfile,
            Dancers = parentProfile.Dancers.ToList(),
            EnrolledClasses = familyEnrollments,
            
            // Mock data for the UI hub until we build the admin tools for these
            Announcements = new List<string>(),
            UpcomingEvents = new List<string>(),
            CompetitionPlacements = new List<string>()
        };

        return View(viewModel);
    }
    
    // GET
    /// <summary>
    /// Renders the form for adding a new dancer to the parent's profile.
    /// The form allows input of dancer details such as first name, last name, date of birth, and skill level.
    /// </summary>
    /// <returns>A view representing the add dancer form with an empty model.</returns>
    
    public IActionResult AddDancer()
    {
        return View(new AddDancerViewModel { FirstName = "", LastName = "" });
    }
    
    // POST 
    /// <summary>
    /// Adds a new dancer associated with the authenticated parent's profile,
    /// using the provided input data and ensuring model validation.
    /// </summary>
    /// <param name="input">The data required to create a new dancer, including first name, last name, date of birth, and skill level.</param>
    /// <returns>A task that represents the asynchronous operation. Returns a redirect to the dashboard upon success, or the input form view on validation failure.</returns>
    
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