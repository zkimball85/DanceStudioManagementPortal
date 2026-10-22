using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EatonvilleDanceStudio.Data;

namespace EatonvilleDanceStudio.Models;

/// <summary>
/// Represents a block of time for private lessons at the dance studio.
/// This class includes details such as the instructor, time, date,
/// booking status, and optionally the student's name if the block is booked.
/// </summary>

public class PrivateLessonBlock
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string InstructorId { get; set; } = string.Empty;
    
    [ForeignKey("InstructorId")]
    public ApplicationUser? Instructor { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    
    [Required]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }
    
    [Required]
    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; }
    
    // Flag to indicate if a parent has claimed this time slot
    public bool IsBooked { get; set; } = false;
    
    //Optional: The name of the student who booked this time slot, can be null if open.
    [StringLength(100)]
    public string? StudentName { get; set; }
}