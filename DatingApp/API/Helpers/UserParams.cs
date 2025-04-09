namespace API.Helpers;

public class UserParams : PaginationParams
{
    public string? Gender { get; set; }
    public string? CurrentUsername { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 60;
    public List<string>? Interests { get; set; }
    public string OrderBy { get; set; } = "lastActive";
}