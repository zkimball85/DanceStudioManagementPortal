using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EatonvilleDanceStudio.Models;

public class Dancer
{
    [Key]
    public int Id { get; set; }
    
    // Foreign key linking the dancer to the parent
    [Required]
    public int ParentProfileId { get; set; }
    
    [ForeignKey("ParentProfileId")]
    public ParentProfile Parent { get; set; }
    
    [Required]
    [StringLength(50)]
    [Display(Name = "Dancer First Name")]
    public required string FirstName { get; set; }
    
    [Required]
    [StringLength(50)]
    [Display(Name = "Dancer Last Name")]
    public required string LastName { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Dancer Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    // --- New Properties for Issue #16 ---
    [Required]
    [StringLength(30)]
    [Display(Name = "Skill Level")]
    public string SkillLevel { get; set; } = "Beginner"; // e.g., Beginner, Intermediate, Advanced

    public string? ProfilePhotoPath { get; set; }
    
    [StringLength(20)]
    public string PhotoApprovalStatus { get; set; } = "None"; // None, Pending, Approved, Rejected
}