using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Otis.Users;

public class UserService
{
    private const string PreregisterBucketName = "otis.user.preregister";

    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IS3Client s3Client;

    public UserService()
    {
        this.s3Client = new S3Client();
    }

    public UserService(IS3Client s3Client)
    {
        this.s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
    }

    // TODO consider making this method async
    public APIGatewayProxyResponse PreregisterUser(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            var json = JsonSerializer.Deserialize<UserRegisterRequestModel>(request.Body, jsonSerializerOptions);

            if (!ModelValidator.IsValid(json, out var errors))
            {
                context.Logger.LogWarning($"Invalid input: Body = \"{request.Body}\" ; Errors: {string.Join(",", errors)}");

                return new APIGatewayProxyResponse
                {
                    Body = "Invalid input",
                    StatusCode = 400
                };
            }

            string keyName = GetKeyName();
            s3Client.UploadFileToBucket(
                bucketName: PreregisterBucketName,
                keyName: keyName,
                content: JsonSerializer.Serialize(json)).GetAwaiter().GetResult();

            context.Logger.LogInformation($"Pre-register request received and uploaded with key {keyName}");

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

    private string GetKeyName() => $"UserPreregister_{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff")}.json";
}