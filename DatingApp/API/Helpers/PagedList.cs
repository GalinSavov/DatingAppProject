using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class PagedList<T> : List<T>
{
    public int TotalCount { get; set; }
    public int CurrentPageNumber { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        CurrentPageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int pageSize, int pageNumber)
    {
        var count = await query.CountAsync();
        var items = await query.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}