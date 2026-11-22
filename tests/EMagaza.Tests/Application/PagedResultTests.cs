using EMagaza.Application.Models;

namespace EMagaza.Tests.Application;

public class PagedResultTests
{
    private static PagedResult<int> Result(int total, int page, int size = 12) =>
        new(Array.Empty<int>(), total, page, size);

    [Theory]
    [InlineData(0, 1)]
    [InlineData(12, 1)]
    [InlineData(13, 2)]
    [InlineData(25, 3)]
    public void TotalPages_RoundsUp(int total, int expectedPages) =>
        Assert.Equal(expectedPages, Result(total, 1).TotalPages);

    [Fact]
    public void FirstPage_HasNoPrevious_ButHasNext()
    {
        var page = Result(25, 1);

        Assert.False(page.HasPrevious);
        Assert.True(page.HasNext);
    }

    [Fact]
    public void LastPage_HasPrevious_ButNoNext()
    {
        var page = Result(25, 3);

        Assert.True(page.HasPrevious);
        Assert.False(page.HasNext);
    }
}
