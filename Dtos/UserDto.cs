namespace UserVault.Dtos;

public class UserDto
{
    public string Login { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public bool IsActive { get; set; }
}