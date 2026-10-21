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

    // GET: /Parent/Dashboard **Gemini Supplied Code**

    /// <summary>
    /// Displays the dashboard for the parent user, providing an overview of their profile, associated dancers,
    /// enrolled dance classes, recent ledger transactions, and current account balance.
    /// The data is synthesized into a single view model for easy visualization and interaction.
    /// </summary>
    /// <returns>A view containing detailed information about the parent's profile, dancers,
    /// enrollments, and account activity.</returns>
    
    public async Task<IActionResult> Dashboard()
    {
        // 1. Get the currently logged-in parent
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        // 2. Fetch ParentProfile including Dancers
        var parentProfile = await _context.ParentProfiles
            .Include(p => p.Dancers)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (parentProfile == null) return NotFound("Parent profile not found");

        // 3. Fetch enrollments for the parent's dancers
        var dancerIds = parentProfile.Dancers.Select(d => d.Id).ToList();
        var enrolledClasses = await _context.Enrollments
            .Include(e => e.DanceClass)
            .Include(e => e.Dancer)
            .Where(e => dancerIds.Contains(e.DancerId))
            .ToListAsync();

        // 4. Fetch all ledger entries for this parent, newest first
        var ledgerEntries = await _context.LedgerEntries
            .Where(l => l.ParentId == userId)
            .OrderByDescending(l => l.TransactionDate)
            .ToListAsync();

        // 5. Instantiate the View Model and populate our split lists and calculated balance
        var viewModel = new ParentDashboardViewModel
        {
            Profile = parentProfile,
            Dancers = parentProfile.Dancers.ToList(),
            EnrolledClasses = enrolledClasses,
            CurrentBalance = ledgerEntries.Sum(l => l.Amount),
            RecentCharges = ledgerEntries.Where(l => l.Amount > 0).ToList(),
            RecentPayments = ledgerEntries.Where(l => l.Amount <= 0).ToList()
        };

        // 6. Pass the heavily-lifed data straight to the View
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