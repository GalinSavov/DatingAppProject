namespace API.Entities;

public class AppUserInterest
{
    public int Id { get; set; }
    public AppUser User { get; set; } = null!;
    public int UserId { get; set; }
    public Interest Interest { get; set; } = null!;
    public int InterestId { get; set; }
}