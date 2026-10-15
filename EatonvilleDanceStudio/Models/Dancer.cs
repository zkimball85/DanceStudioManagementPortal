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
    [Display(Name = "Dancer First Name")]
    public string FirstName { get; set; }
    
    [Required]
    [Display(Name = "Dancer Last Name")]
    public string LastName { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Dancer Date of Birth")]
    public DateTime? DateOfBirth { get; set; }
}