using Fiap.Soat.Hackaton.ProcessingService.Domain.Shared;
using System.Net;

namespace Fiap.Soat.Hackaton.ProcessingService.Domain.Tests.Shared;

public class ResponseTests
{
    [Test]
    public void ResponseFactoryOk_ShouldCreateSuccessResponse()
    {
        var response = ResponseFactory.Ok("payload", HttpStatusCode.Accepted);

        Assert.That(response.IsSuccess, Is.True);
        Assert.That(response.Data, Is.EqualTo("payload"));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
    }

    [Test]
    public void ResponseFactoryFail_ShouldCreateFailureResponse()
    {
        var response = ResponseFactory.Fail<string>("invalid input", HttpStatusCode.BadRequest);

        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(response.Reasons.Select(reason => reason.Message), Contains.Item("invalid input"));
    }

    [Test]
    public void ResponseImplicitConversionToNonGenericResponse_ShouldPreserveStatusAndReasons()
    {
        Response<string> source = ResponseFactory.Fail<string>("boom", HttpStatusCode.NotFound);

        Response converted = source;

        Assert.That(converted.IsSuccess, Is.False);
        Assert.That(converted.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(converted.Reasons.Select(reason => reason.Message), Contains.Item("boom"));
    }

    [Test]
    public void ResponseFactoryFailFromArray_ShouldCreateFailureResponse()
    {
        var response = ResponseFactory.Fail(new[] { "a", "b" }, HttpStatusCode.InternalServerError);

        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(response.Reasons.Count, Is.EqualTo(2));
    }
}

