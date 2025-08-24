using Domain.Entities.Users;

namespace Domain.Entities.Teams;

public class Team : Entity
{
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<ApplicationUser>? Users { get; set; }
}