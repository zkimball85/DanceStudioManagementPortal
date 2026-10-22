using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EatonvilleDanceStudio.Models;

/// <summary>
/// Represents an attendance record for a dancer's enrolled class.
/// </summary>
/// <remarks>
/// This class is used to capture attendance information for a specific enrolled dancer,
/// including the class date, attendance status, and optional instructor notes.
/// It associates with the Enrollment class to identify the dancer and class details.
/// </remarks>

public class AttendanceRecord
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int EnrollmentId { get; set; }
    
    [ForeignKey("EnrollmentId")]
    public Enrollment? Enrollment { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Class Date")]
    public DateTime Date { get; set; }
    
    // Options: Present, Absent, Excused, Tardy
    [Required]
    [StringLength(20)]
    [Display(Name = "Attendance Status")]
    public string Status { get; set; } = "Present";
    
    [StringLength(250)]
    [Display(Name = " Instructor Notes")]
    public string? Notes { get; set; }
}