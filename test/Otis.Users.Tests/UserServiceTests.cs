using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FluentAssertions;
using Moq;
using Xunit;

namespace Otis.Users
{
    public class UserServiceTests
    {
        private readonly UserService subject = new();
        private readonly Mock<ILambdaContext> lamdaContextMock = new();

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

            lamdaContextMock.Setup(m => m.Logger.LogInformation(It.IsRegex("john.doe@xpto.com")));

            // ACT
            var response = subject.PreregisterUser(request, lamdaContextMock.Object);

            // ASSERT
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);

            lamdaContextMock.VerifyAll();
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

            lamdaContextMock.VerifyAll();
        }
    }
}
