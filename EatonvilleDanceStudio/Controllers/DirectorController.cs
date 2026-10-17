using EatonvilleDanceStudio.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EatonvilleDanceStudio.Controllers;

[Authorize(Roles = "Director")]
public class DirectorController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public DirectorController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    //Get /Director/PendingAccounts
    public async Task<IActionResult> PendingAccounts()
    {
        // Fetch all the profiles where the status is still pending
        var pending = await _context.ParentProfiles
            .Where(p => p.AccountStatus == "Pending")
            .ToListAsync();

        return View(pending);
    }
    
    // Post: /Director/Approve
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var profile = await _context.ParentProfiles.FindAsync(id);
        if (profile != null)
        {
            profile.AccountStatus = "Approved";
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(PendingAccounts));
    }
}