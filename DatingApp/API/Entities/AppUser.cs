using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;
public class AppUser : IdentityUser<int>
{
    public DateOnly DateOfBirth { get; set; }
    public required string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public required string Gender { get; set; }
    public string? Introduction { get; set; }
    public List<AppUserInterest> UserInterests { get; set; } = [];
    public string? LookingFor { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public List<Photo> Photos { get; set; } = [];

    // navigation properties : retrieve a list of likes or messages from an user query
    public List<UserLike> LikedByOtherUsers { get; set; } = [];
    public List<UserLike> LikedUsers { get; set; } = [];

    public List<Message> SentMessages { get; set; } = [];
    public List<Message> ReceivedMessages { get; set; } = [];
    public List<AppUserRole> UserRoles { get; set; } = [];
    public int GetAge()
    {
        return DateOfBirth.CalculateAge();
    }
}
