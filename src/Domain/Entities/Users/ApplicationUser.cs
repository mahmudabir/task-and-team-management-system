using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Domain.Entities.Teams;

using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Users;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }

    public long TeamId { get; set; }
    public Team? Team { get; set; }

    [NotMapped]
    public IList<string>? Roles { get; set; }

    [JsonIgnore]
    public List<RefreshToken> RefreshTokens { get; set; }
}