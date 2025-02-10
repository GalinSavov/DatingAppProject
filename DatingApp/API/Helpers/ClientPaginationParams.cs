namespace API.Helpers;

public class ClientPaginationParams
{
    private const int MAX_ITEMS_PER_PAGE = 50;
    public int CurrentPageNumber { get; set; } = 1;
    private int itemsPerPage = 10;

    public int ItemsPerPage
    {
        get => itemsPerPage;
        set => itemsPerPage = (value > MAX_ITEMS_PER_PAGE) ? MAX_ITEMS_PER_PAGE : value;
    }
}