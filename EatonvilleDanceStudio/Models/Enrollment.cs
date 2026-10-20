using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EatonvilleDanceStudio.Models;

/// <summary>
/// Represents the enrollment of a dancer into a dance class.
/// </summary>
public class Enrollment
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int DancerId { get; set; }
    
    [ForeignKey("DancerId")]
    public Dancer? Dancer { get; set; }
    
    [Required]
    public int DanceClassId { get; set; }
    
    [ForeignKey("DanceClassId")]
    public DanceClass? DanceClass { get; set; }
    
    [Required]
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    
    [Required, StringLength(20)]
    public string Status { get; set; } = "Enrolled"; // Enrolled, Waitlisted, Dropped

}