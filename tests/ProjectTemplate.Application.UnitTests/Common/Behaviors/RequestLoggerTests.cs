using Microsoft.Extensions.Logging;
using Moq;
using ProjectTemplate.Application.Common.Behaviors;
using ProjectTemplate.Application.Common.Interfaces;
using Xunit;

namespace ProjectTemplate.Application.UnitTests.Common.Behaviors;

public class RequestLoggerTests
{
    private Mock<ILogger<Request>> _loggerMock;
    private Mock<IUser> _userMock;
    private Mock<IIdentityService> _identityServiceMock;

    public RequestLoggerTests()
    {
        _loggerMock = new Mock<ILogger<Request>>();
        _userMock = new Mock<IUser>();
        _identityServiceMock = new Mock<IIdentityService>();
    }

    [Fact]
    public async Task Should_LogsExpectedInformation_When_UserIsAuthenticated()
    {
        // Arrange
        string userId = Guid.NewGuid().ToString();
        string userName = "TestUser";
        _userMock.Setup(u => u.Id).Returns(userId);
        _identityServiceMock.Setup(s => s.GetUserNameAsync(userId)).ReturnsAsync(userName);
        _loggerMock.Setup(l => l.Log(LogLevel.Information,
                                It.IsAny<EventId>(),
                                It.Is<It.IsAnyType>((v, t) => true),
                                It.IsAny<Exception>(),
                                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)))
                   .Verifiable();

        var request = new Request(Guid.NewGuid().ToString(), description: "Request test");
        var loggingBehavior = new LoggingBehavior<Request>(_loggerMock.Object, _userMock.Object, _identityServiceMock.Object);

        // Act
        await loggingBehavior.Process(request, CancellationToken.None);

        // Assert                
        _identityServiceMock.Verify(s => s.GetUserNameAsync(It.IsAny<string>()), Times.Once);
        _loggerMock.Verify(l => l.Log(LogLevel.Information,
                                It.IsAny<EventId>(),
                                It.Is<It.IsAnyType>((v, t) => true),
                                It.IsAny<Exception>(),
                                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
    }

    [Fact]
    public async Task ShouldNot_CallGetUserNameAsync_When_UserIsUnauthenticated()
    {
        // Arrange        
        _userMock.Setup(u => u.Id).Returns(string.Empty);
        var loggingBehavior = new LoggingBehavior<Request>(_loggerMock.Object, _userMock.Object, _identityServiceMock.Object);
        _loggerMock.Setup(l => l.Log(LogLevel.Information,
                                It.IsAny<EventId>(),
                                It.Is<It.IsAnyType>((v, t) => true),
                                It.IsAny<Exception>(),
                                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)))
                   .Verifiable();

        // Act
        await loggingBehavior.Process(new Request(string.Empty, string.Empty), CancellationToken.None);

        // Assert        
        _identityServiceMock.Verify(s => s.GetUserNameAsync(It.IsAny<string>()), Times.Never);
        _loggerMock.Verify(l => l.Log(LogLevel.Information,
                                It.IsAny<EventId>(),
                                It.Is<It.IsAnyType>((v, t) => true),
                                It.IsAny<Exception>(),
                                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
    }
}
