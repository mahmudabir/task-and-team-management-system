using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Users;

public class User
{
    public string? Id { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Username { get; set; }


    [Required]
    public string FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Password { get; set; }

    public List<string> Roles { get; set; } = [];

    public bool IsLocked { get; set; }

    public long TeamId { get; set; }
}