namespace Media.Core.Dtos.Exchange
{
    /// <summary>
    /// Input for pagination objects.
    /// </summary>
    /// <param name="PageNumber">The page number to find.</param>
    /// <param name="PageSize">The size the pages can be.</param>
    public record PaginationReq(int PageNumber = 1, int PageSize = 10);

    /// <summary>
    /// Output for pagination objects.
    /// </summary>
    /// <param name="PageNumber">The page number to find.</param>
    /// <param name="PageSize">The size the pages can be.</param>
    /// <param name="TotalItems">The total items that exist.</param>
    /// <param name="TotalPages">The total amount of pages.</param>
    public record PaginationRes(int PageNumber, int PageSize, int TotalItems, int TotalPages);
}
