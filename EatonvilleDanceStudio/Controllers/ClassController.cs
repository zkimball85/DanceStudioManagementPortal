using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EatonvilleDanceStudio.Data;
using EatonvilleDanceStudio.Models;
using EatonvilleDanceStudio.ViewModels;

namespace EatonvilleDanceStudio.Controllers;

[Authorize]
public class ClassController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClassController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Class
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var parentProfile = await _context.ParentProfiles.Include(p => p.Dancers).FirstOrDefaultAsync(p => p.UserId == userId);
        
        if (parentProfile == null) return NotFound("Parent profile not found");

        if (parentProfile.AccountStatus != "Approved")
        {
            return Unauthorized("Your account must be approved by the studio director before you can browse classes.");
        }

        var classes = await _context.DanceClasses.ToListAsync();

        var viewModel = new ClassBrowseViewModel
        {
            AvailableClasses = classes,
            MyDancers = parentProfile.Dancers.ToList()
        };
        
        return View(viewModel);
    }
    
    // POST:

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int classId, int dancerId)
    {
        var userId = _userManager.GetUserId(User);
        
        // Security Check
        var parentProfile = await _context.ParentProfiles
            .Include(p => p.Dancers)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (parentProfile == null || !parentProfile.Dancers.Any(d => d.Id == dancerId))
        {
            return Unauthorized("You do not have permission to enroll this dancer.");
        }

        if (parentProfile.AccountStatus != "Approved")
        {
            return Unauthorized("Your account must be approved by the studio director before you can enroll in classes.");
        }
        
        // Prevent double enrollment

        var alreadyEnrolled =
            await _context.Enrollments.AnyAsync(e => e.DanceClassId == classId && e.DancerId == dancerId);

        if (alreadyEnrolled)
        {
            TempData["ErrorMessage"] = "Dancer is already enrolled in this class.";
            return RedirectToAction(nameof(Index));
        }
        
        //Create enrollment

        var enrollment = new Enrollment
        {
            DanceClassId = classId,
            DancerId = dancerId,
            Status = "Enrolled"
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Dancer enrolled successfully.";
        return RedirectToAction(nameof(Index));
    }
}