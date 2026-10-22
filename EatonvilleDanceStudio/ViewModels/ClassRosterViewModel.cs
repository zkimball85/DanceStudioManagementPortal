using EatonvilleDanceStudio.Models;

namespace EatonvilleDanceStudio.ViewModels;

/// <summary>
/// Represents the view model for managing the roster of a specific dance class on a particular date.
/// This view model is designed to provide the essential data needed for creating or displaying
/// an attendance sheet for a class.
/// </summary>

public class ClassRosterViewModel
{
    // The specific class we are viewing
    public required DanceClass DanceClass { get; set; }
    
    // The date for this specific attendance sheet
    public DateTime ClassDate { get; set; } = DateTime.Today;
    
    // A list containing an attendance record for every enrolled dancer
    public List<AttendanceRecord> AttendanceList { get; set; } = new();
}