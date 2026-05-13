using Fiap.Soat.Hackaton.ProcessingService.Domain.Shared;

namespace Fiap.Soat.Hackaton.ProcessingService.Domain.Tests.Shared;

public class PaginateTests
{
    [Test]
    public void Constructor_ShouldExposeProvidedValues()
    {
        var items = new[] { "a", "b" };

        var sut = new Paginate<string>(items, totalCount: 2, pageSize: 10, currentPage: 1, totalPages: 1);

        Assert.That(sut.Items, Is.EqualTo(items));
        Assert.That(sut.TotalCount, Is.EqualTo(2));
        Assert.That(sut.PageSize, Is.EqualTo(10));
        Assert.That(sut.CurrentPage, Is.EqualTo(1));
        Assert.That(sut.TotalPages, Is.EqualTo(1));
    }

    [Test]
    public void PaginatedRequest_ShouldUseDefaultValues()
    {
        var sut = new PaginatedRequest();

        Assert.That(sut.PageNumber, Is.EqualTo(1));
        Assert.That(sut.PageSize, Is.EqualTo(15));
    }

    [Test]
    public void PaginatedRequestConstructor_ShouldOverrideDefaults()
    {
        var sut = new PaginatedRequest(3, 50);

        Assert.That(sut.PageNumber, Is.EqualTo(3));
        Assert.That(sut.PageSize, Is.EqualTo(50));
    }
}

