namespace API.Helpers;

public class UserParams
{
    private const int MAX_ITEMS_PER_PAGE = 50;
    public int CurrentPageNumber { get; set; } = 1;
    private int itemsPerPage = 10;

    public int ItemsPerPage
    {
        get => itemsPerPage;
        set => itemsPerPage = (value > MAX_ITEMS_PER_PAGE) ? MAX_ITEMS_PER_PAGE : value;
    }
    public string? Gender { get; set; }
    public string? CurrentUsername { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 60;
}