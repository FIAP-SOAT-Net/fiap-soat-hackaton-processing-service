using Fiap.Soat.Hackaton.ProcessingService.Application.Mappers;
using Fiap.Soat.Hackaton.ProcessingService.Domain.Shared;
using System.Net;

namespace Fiap.Soat.Hackaton.ProcessingService.Application.Tests.Mappers;

public class ResponseMapperTests
{
    [Test]
    public void Map_ShouldConvertData_WhenInputResponseIsSuccess()
    {
        var response = ResponseFactory.Ok(10, HttpStatusCode.Accepted);

        var mapped = ResponseMapper.Map(response, value => value.ToString());

        Assert.That(mapped.IsSuccess, Is.True);
        Assert.That(mapped.Data, Is.EqualTo("10"));
        Assert.That(mapped.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
    }

    [Test]
    public void Map_ShouldKeepError_WhenInputResponseIsFailure()
    {
        var response = ResponseFactory.Fail<int>("invalid input", HttpStatusCode.BadRequest);

        var mapped = ResponseMapper.Map(response, value => value.ToString());

        Assert.That(mapped.IsSuccess, Is.False);
        Assert.That(mapped.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(mapped.Reasons.Select(reason => reason.Message), Contains.Item("invalid input"));
    }

    [Test]
    public void MapPaginate_ShouldConvertItems_WhenInputResponseIsSuccess()
    {
        var paginate = new Paginate<int>(new[] { 1, 2, 3 }, 3, 10, 1, 1);
        var response = ResponseFactory.Ok(paginate, HttpStatusCode.OK);

        var mapped = ResponseMapper.Map<int, string>(response, item => $"file-{item}");

        Assert.That(mapped.IsSuccess, Is.True);
        Assert.That(mapped.Data.Items.ToArray(), Is.EqualTo(new[] { "file-1", "file-2", "file-3" }));
        Assert.That(mapped.Data.TotalCount, Is.EqualTo(3));
    }

    [Test]
    public void MapPaginate_ShouldKeepError_WhenInputResponseIsFailure()
    {
        var response = ResponseFactory.Fail<Paginate<int>>("query failed", HttpStatusCode.InternalServerError);

        var mapped = ResponseMapper.Map<int, string>(response, item => item.ToString());

        Assert.That(mapped.IsSuccess, Is.False);
        Assert.That(mapped.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(mapped.Reasons.Select(reason => reason.Message), Contains.Item("query failed"));
    }
}
