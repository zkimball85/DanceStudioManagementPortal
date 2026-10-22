using EatonvilleDanceStudio.Data;
using EatonvilleDanceStudio.Models;

namespace EatonvilleDanceStudio.ViewModels;

/// <summary>
/// Represents the data model for the instructor's dashboard view.
/// This view model encapsulates information about the instructor,
/// their assigned classes, today's schedule, and the upcoming schedule.
/// </summary>

public class InstructorDashboardViewModel
{
    public required ApplicationUser Instructor { get; set; }
    public List<DanceClass> AssignedClasses { get; set; } = new();
    public List<DanceClass> TodaysSchedule { get; set; } = new();
    public List<DanceClass> UpcomingSchedule { get; set; } = new();
    
    // A blank log used to bind the form inputs
    public InstructorTimeLog NewLog { get; set; } = new();
    
    // A history of the instructor's recently logged hours
    public List<InstructorTimeLog> RecentTimeLogs { get; set; } = new();
    
    // For submitting a new open time slot
    public PrivateLessonBlock NewLessonBlock { get; set; } = new();
    
    // A list of upcoming private lesson blocks, both booked and open.
    public List<PrivateLessonBlock> PrivateLessons { get; set; } = new();
}