using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EatonvilleDanceStudio.Models;
using EatonvilleDanceStudio.ViewModels;
using EatonvilleDanceStudio.Data;

namespace EatonvilleDanceStudio.Controllers;

public class InstructorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public InstructorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    //GET

    /// <summary>
    /// Displays the dashboard view for an instructor. The method retrieves the
    /// currently logged-in instructor's details and their assigned classes.
    /// It organizes the information into schedules for classes happening today
    /// and upcoming classes.
    /// </summary>
    /// <returns>The instructor's dashboard view populated with their details and class schedules.</returns>
    
    public async Task<IActionResult> Dashboard()
    {
        // Identify current logged in Instructor
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();
        
        var instructor = await _userManager.FindByIdAsync(userId);
        if (instructor == null) return NotFound();
        
        // Fetch all classes assigned to this Instructor
        var assignedClasses = await _context.DanceClasses
            .Where(c => c.InstructorId == userId)
            .ToListAsync();

        // FIX 1: Determine today's day of the week so the LINQ query below knows what 'today' is
        var today = DateTime.Today.DayOfWeek.ToString();
        
        // Fetch upcoming private lesson slots today or in the future
        var privateLessons = await _context.PrivateLessonBlocks
            .Where(p => p.InstructorId == userId && p.Date >= DateTime.Today)
            .OrderBy(p => p.Date)
            .ThenBy(p => p.StartTime)
            .ToListAsync();

        // FIX 2: Fetch the instructor's 10 most recent time logs
        var recentLogs = await _context.InstructorTimeLogs
            .Where(t => t.InstructorId == userId)
            .OrderByDescending(t => t.Date)
            .Take(10)
            .ToListAsync();
        
        // Fill the ViewModel buckets
        var viewModel = new InstructorDashboardViewModel
        {
            Instructor = instructor,
            AssignedClasses = assignedClasses,
            
            // Classes happening today, ordered by start time.
            TodaysSchedule = assignedClasses
                .Where(c => c.DayOfWeek == today)
                .OrderBy(c => c.StartTime)
                .ToList(),
            
            // The list of upcoming classes, everything that is not happening today
            UpcomingSchedule = assignedClasses
                .Where(c => c.DayOfWeek != today)
                .OrderBy(c => c.DayOfWeek)
                .ThenBy(c => c.StartTime)
                .ToList(),

            // FIX 3: Actually pass the queried data into the View Model so the HTML can see it
            PrivateLessons = privateLessons,
            RecentTimeLogs = recentLogs
        };
        
        return View(viewModel);
    }
    
    // POST: /Instructor/LogHours

    /// <summary>
    /// Logs hours worked by the instructor. The method validates the
    /// submitted data and creates a new time log entry for the instructor.
    /// If the input is valid, the hours are saved and submitted for
    /// approval; otherwise, an error message is displayed.
    /// </summary>
    /// 
    /// <param name="model">The view model containing the instructor's
    /// input for logging worked hours, including the date, number of
    /// hours, and an optional description.</param>
    /// 
    /// <returns>The updated instructor dashboard view with a success
    /// or error message.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogHours(InstructorDashboardViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        // Basic validation to ensure they didn't submit 0 hours or a blank date
        if (model.NewLog.Hours > 0 && model.NewLog.Date != default)
        {
            var timeLog = new InstructorTimeLog
            {
                InstructorId = userId,
                Date = model.NewLog.Date,
                Hours = model.NewLog.Hours,
                Description = model.NewLog.Description,
                IsApproved = false // Always defaults to false until Director reviews it
            };

            _context.InstructorTimeLogs.Add(timeLog);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Hours successfully logged and sent to the Director for approval.";
        }
        else
        {
            TempData["ErrorMessage"] = "Please enter a valid date and hours worked.";
        }

        return RedirectToAction(nameof(Dashboard));
    }
    
    // POST: /Instructor/AddPrivateLesson

    /// <summary>
    /// Adds a new private lesson block for the instructor. The method ensures the provided
    /// lesson block has a valid date and time range before saving it to the database.
    /// If the addition is successful, a success message is displayed; otherwise,
    /// an error message is returned with validation feedback.
    /// </summary>
    /// <param name="model">The view model containing details for the new private
    /// lesson block, including date, start time, and end time.</param>
    /// <returns>A redirect to the instructor's dashboard view, updated with the
    /// new private lesson block or feedback on input errors.</returns>
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPrivateLesson(InstructorDashboardViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        // Ensure they aren't adding past dates and start time is before end time
        if (model.NewLessonBlock.Date >= DateTime.Today && model.NewLessonBlock.StartTime < model.NewLessonBlock.EndTime)
        {
            var block = new PrivateLessonBlock
            {
                InstructorId = userId,
                Date = model.NewLessonBlock.Date,
                StartTime = model.NewLessonBlock.StartTime,
                EndTime = model.NewLessonBlock.EndTime,
                IsBooked = false // Defaults to false until someone books it
            };

            _context.PrivateLessonBlocks.Add(block);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Private lesson block successfully opened!";
        }
        else
        {
            TempData["ErrorMessage"] = "Invalid date or time range. Make sure the start time is before the end time.";
        }

        return RedirectToAction(nameof(Dashboard));
    }
}