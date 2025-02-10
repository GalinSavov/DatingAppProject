namespace API.Helpers;

public class PaginationHeader(int totalPages, int currentPageNumber, int itemsPerPage, int totalItems)
{
    public int TotalPages { get; set; } = totalPages;
    public int CurrentPageNumber { get; set; } = currentPageNumber;
    public int itemsPerPage { get; set; } = itemsPerPage;
    public int totalItems { get; set; } = totalItems;
}