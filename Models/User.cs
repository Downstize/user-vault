using System.ComponentModel.DataAnnotations;

namespace UserVault.Models;

public class User
{
    [Key]
    public Guid Guid { get; set; } = Guid.NewGuid();

    [Required]
    [RegularExpression("^[a-zA-Z0-9]+$")]
    public string Login { get; set; } = null!;

    [Required]
    [RegularExpression("^[a-zA-Z0-9]+$")]
    public string Password { get; set; } = null!;

    [Required]
    [RegularExpression("^[a-zA-Zа-яА-Я]+$")]
    public string Name { get; set; } = null!;

    public int Gender { get; set; }

    public DateTime? Birthday { get; set; }

    public bool Admin { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; } = null!;

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? RevokedOn { get; set; }

    public string? RevokedBy { get; set; }
}