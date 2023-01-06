using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Otis.Users;

public class UserService
{
    public APIGatewayProxyResponse PreregisterUser(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            var json = JsonSerializer.Deserialize<UserRegisterRequestModel>(request.Body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            if (!ModelValidator.IsValid(json, out var errors))
            {
                context.Logger.LogWarning($"Invalid input: Body = \"{request.Body}\" ; Errors: {string.Join(",", errors)}");

                return new APIGatewayProxyResponse
                {
                    Body = "Invalid input",
                    StatusCode = 400
                };
            }

            context.Logger.LogInformation($"Receive a request from {json.EmailAddress}");

            return new APIGatewayProxyResponse
            {
                StatusCode = 200
            };
        }
        catch (JsonException e)
        {
            context.Logger.LogError($"Content is not a valid JSON: {e}");
            return new APIGatewayProxyResponse
            {
                Body = "Invalid JSON",
                StatusCode = 400
            };
        }
    }
}