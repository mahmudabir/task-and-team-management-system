using Softoverse.CqrsKit.Models.Utility;

namespace Domain.Entities.Teams;

public static class TeamErrors
{
    public static CqrsError AlreadyAdded(string name) => CqrsError.Create("Team.AlreadyAdded",
                                                                           $"The Team with Name = '{name}' is already added.");

    public static CqrsError NotFound(long id) => CqrsError.Create("Team.NotFound",
                                                                  $"The Team with the Id = '{id}' was not found");
}
