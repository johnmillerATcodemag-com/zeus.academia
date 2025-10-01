using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Base entity class providing audit fields for all entities in the system.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    [Required]
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ModifiedBy { get; set; } = string.Empty;
}