namespace Shared.Settings;

public sealed class AdminSettings
{
    public const string SectionName = "AdminSettings";

    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
}


public sealed class ManagerSettings
{
    public const string SectionName = "ManagerSettings";

    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
}

public sealed class EmployeeSettings
{
    public const string SectionName = "EmployeeSettings";

    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
}