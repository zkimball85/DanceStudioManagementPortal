using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EatonvilleDanceStudio.Data;

namespace EatonvilleDanceStudio.Models;

public class ParentProfile
{
    [Key]
    public int Id { get; set; }
    
    //Links the profile to the IdentityUser auth table
    [Required]
    public string UserId { get; set; }
    
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
    
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    
    [Required]
    [Phone]
    [Display(Name = "Primary Number")]
    public string PrimaryPhone { get; set; }
    
    [Required]
    [Display(Name = "Emergency Contact Name")]
    public string EmergencyContactName { get; set; }
    
    [Required]
    [Phone]
    [Display(Name = "Emergency Contact Number")]
    public string EmergencyContactPhone { get; set; }
    
    // Navigation property: a parent can have many dancers.
    public ICollection<Dancer> Dancers { get; set; } = new List<Dancer>();

}