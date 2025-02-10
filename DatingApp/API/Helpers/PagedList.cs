using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class PagedList<T> : List<T>
{
    public int TotalItems { get; set; } // Total number of items in the database
    public int CurrentPageNumber { get; set; } // Current page being viewed
    public int TotalPages { get; set; } // Total number of pages available
    public int ItemsPerPage { get; set; } // Number of items per page
    public PagedList(IEnumerable<T> items, int totalItems, int currentPageNumber, int itemsPerPage)
    {
        CurrentPageNumber = currentPageNumber;
        ItemsPerPage = itemsPerPage;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage);
        AddRange(items);
    }
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int itemsPerPage, int currentPageNumber)
    {
        var itemCount = await query.CountAsync();
        var items = await query.Skip(itemsPerPage * (currentPageNumber - 1)).Take(itemsPerPage).ToListAsync();
        return new PagedList<T>(items, itemCount, currentPageNumber, itemsPerPage);
    }
}