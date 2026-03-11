using Minio;
using Minio.DataModel.Args;

namespace AuthService.Api;

public class MinioBucketInitializer
{
    private readonly IMinioClient _minio;
    private readonly IConfiguration _config;

    public MinioBucketInitializer(IMinioClient minio, IConfiguration config)
    {
        _minio = minio;
        _config = config;
    }

    public async Task InitializeAsync()
    {
        var bucket = _config["Minio:Bucket"];

        bool exists = await _minio.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(bucket)
        );

        if (!exists)
        {
            await _minio.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(bucket)
            );

            Console.WriteLine($"[MinIO] Bucket created: {bucket}");
        }
        else
        {
            Console.WriteLine($"[MinIO] Bucket already exists: {bucket}");
        }
    }
}