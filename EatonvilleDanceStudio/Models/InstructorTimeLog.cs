using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EatonvilleDanceStudio.Data;

namespace EatonvilleDanceStudio.Models;

/// <summary>
/// Represents a logged shift or block of hours worked by an instructor for payroll purposes.
/// </summary>

public class InstructorTimeLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string InstructorId { get; set; } = string.Empty;

    [ForeignKey("InstructorId")]
    public ApplicationUser? Instructor { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date Worked")]
    public DateTime Date { get; set; }

    [Required]
    [Range(0.25, 16.0, ErrorMessage = "Hours must be between 0.25 and 16.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Hours Worked")]
    public decimal Hours { get; set; }

    [StringLength(200)]
    [Display(Name = "Notes / Description")]
    public string? Description { get; set; } 

    // The Director will flip this to true when they run payroll
    public bool IsApproved { get; set; } = false; 
}