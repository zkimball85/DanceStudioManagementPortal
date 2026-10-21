using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EatonvilleDanceStudio.Data;

namespace EatonvilleDanceStudio.Models;

// Gemini Supplied Code

/// <summary>
/// Represents a single financial transaction (charge or payment) for a parent's account.
/// </summary>
public class LedgerEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ParentId { get; set; } = string.Empty;

    [ForeignKey("ParentId")]
    public ApplicationUser? Parent { get; set; }

    /// <summary>
    /// The date and time the transaction occurred or was posted.
    /// </summary>
    [Required]
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// A human-readable description (e.g., "November Tuition", "Costume Fee", "Stripe Payment").
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The financial amount. 
    /// Charges (fees/tuition) will be stored as positive values.
    /// Payments (credits) will be stored as negative values.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Categorizes the transaction for easier filtering and reporting.
    /// Values could be: "Tuition", "Costume", "Event", "Payment"
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;
}