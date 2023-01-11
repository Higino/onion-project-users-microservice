using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace Otis.Users;

public interface IS3Client
{
    Task UploadFileToBucket(string bucketName, string keyName, string content);
}

public sealed class S3Client : IS3Client
{
    private readonly IAmazonS3 s3Client;

    public S3Client()
    {
        // TODO Receive configuration
        this.s3Client = new AmazonS3Client(RegionEndpoint.EUWest1);
    }

    public async Task UploadFileToBucket(string bucketName, string keyName, string content)
        => await s3Client.PutObjectAsync(
            new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                ContentBody = content
            });
}