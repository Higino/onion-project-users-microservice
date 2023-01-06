using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FluentAssertions;
using Moq;
using Xunit;

namespace Otis.Users
{
    public class UserServiceTests : IDisposable
    {
        private readonly Mock<ILambdaContext> lamdaContextMock = new(MockBehavior.Strict);
        private readonly Mock<IS3Client> s3ClientMock = new(MockBehavior.Strict);
        private readonly UserService subject;

        public UserServiceTests()
        {
            this.subject = new UserService(s3ClientMock.Object);
        }

        public void Dispose()
        {
            lamdaContextMock.VerifyAll();
            s3ClientMock.VerifyAll();
        }

        [Theory]
        [InlineData("{\"firstName\":\"John\", \"lastName\": \"Doe\", \"emailAddress\": \"john.doe@xpto.com\" }")]
        [InlineData("{\"FirstName\":\"John\", \"LastName\": \"Doe\", \"EmailAddress\": \"john.doe@xpto.com\" }")]
        public void Preregister_Success(string body)
        {
            // ARRANGE 
            var request = new APIGatewayProxyRequest
            {
                Body = body,
            };

            lamdaContextMock.Setup(m => m.Logger.LogInformation(It.Is<string>(message => message.Contains("received and uploaded"))));
            s3ClientMock.Setup(m => m.UploadFileToBucket(
                    "otis.user.preregister",
                    It.IsRegex(@"^UserPreregister_\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}.json$"),
                    It.Is<string>(body => body.Contains("John") && body.Contains("Doe") && body.Contains("john.doe@xpto.com")))
                ).Returns(Task.CompletedTask);

            // ACT
            var response = subject.PreregisterUser(request, lamdaContextMock.Object);

            // ASSERT
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
        }

        [Theory]
        [InlineData("{\"firstName\":\"John\", \"lastName\": \"Doe\", \"emailAddress\": \"not_an_email\" }", "The EmailAddress field is not a valid e-mail address.")]
        [InlineData("{\"firstName\":\"ab\", \"lastName\": \"Doe\", \"emailAddress\": \"not_an_email\" }", "The field FirstName must be a string or array type with a minimum length of '3'.")]
        public void Preregister_Invalid(string body, string expectedErrorMessage)
        {
            // ARRANGE 
            var request = new APIGatewayProxyRequest
            {
                Body = body,
            };

            // Should log a warning
            lamdaContextMock.Setup(m => m.Logger.LogWarning(It.IsRegex(expectedErrorMessage)));

            // ACT
            var response = subject.PreregisterUser(request, lamdaContextMock.Object);

            // ASSERT
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(400);
        }
    }
}
