using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Infrastructure.Entities;

/// <summary>
/// Entity representing an award given to a student.
/// Part of Task 4: Academic Record Management.
/// </summary>
public class Award : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the award.
    /// </summary>
    [Key]
    public new int Id { get; set; }

    /// <summary>
    /// Gets or sets the student's employee number.
    /// </summary>
    [Required]
    public int StudentEmpNr { get; set; }

    /// <summary>
    /// Gets or sets the type of award.
    /// </summary>
    public AwardType AwardType { get; set; }

    /// <summary>
    /// Gets or sets the name of the award.
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the award.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the monetary value of the award (if applicable).
    /// </summary>
    [Range(0, 999999.99)]
    public decimal? MonetaryValue { get; set; }

    /// <summary>
    /// Gets or sets the currency of the monetary value.
    /// </summary>
    [MaxLength(3)]
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets the date the award was given.
    /// </summary>
    public DateTime AwardDate { get; set; }

    /// <summary>
    /// Gets or sets the academic term when the award was given.
    /// </summary>
    public int? AcademicTermId { get; set; }

    /// <summary>
    /// Gets or sets the academic year when the award was given.
    /// </summary>
    public int AcademicYear { get; set; }

    /// <summary>
    /// Gets or sets the organization that gave the award.
    /// </summary>
    [MaxLength(150)]
    public string? AwardingOrganization { get; set; }

    /// <summary>
    /// Gets or sets the criteria that were met to receive the award.
    /// </summary>
    [MaxLength(500)]
    public string? Criteria { get; set; }

    /// <summary>
    /// Gets or sets whether this award appears on transcripts.
    /// </summary>
    public bool AppearsOnTranscript { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this is a recurring award.
    /// </summary>
    public bool IsRecurring { get; set; } = false;

    /// <summary>
    /// Gets or sets the frequency of recurrence (if applicable).
    /// </summary>
    [MaxLength(50)]
    public string? RecurrenceFrequency { get; set; }

    /// <summary>
    /// Gets or sets the certificate or document number.
    /// </summary>
    [MaxLength(50)]
    public string? CertificateNumber { get; set; }

    /// <summary>
    /// Gets or sets additional notes about the award.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether the award is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    [ForeignKey(nameof(StudentEmpNr))]
    public virtual Student Student { get; set; } = null!;

    /// <summary>
    /// Navigation property to the academic term.
    /// </summary>
    [ForeignKey(nameof(AcademicTermId))]
    public virtual AcademicTerm? AcademicTerm { get; set; }
}