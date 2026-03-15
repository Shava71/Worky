using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace AuthService.Application.Controllers;

[ApiController]
[Route("api/Auth/profile-photo")]
public class ProfilePhotoController : Controller
{
    private readonly IMinioClient _minio;
    private readonly IConfiguration _config;

    public ProfilePhotoController(IMinioClient minio, IConfiguration config)
    {
        _minio = minio;
        _config = config;
    }

    private string Bucket => _config["Minio:Bucket"];

    private string GetObjectName(Guid userId)
    {
        return $"{userId}.webp";
    }
    private bool CheckUserId(Guid userId)
    {
        Guid currentUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return currentUser == userId;
    }
    // ----------------------------
    // Получить ссылку на фото
    // GET api/Auth/profile-photo/{userId}
    // ----------------------------
    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(Guid userId)
    {
        var objectName = GetObjectName(userId);
        var url = await _minio.PresignedGetObjectAsync(
            new PresignedGetObjectArgs()
                .WithBucket(Bucket)
                .WithObject(objectName)
                .WithExpiry(60 * 60) // 1 час
        );
        return Ok(new { url });
    }

    // ----------------------------
    // Получить ссылку для загрузки фото
    // POST api/Auth/profile-photo/upload/{userId}
    // ----------------------------
    [HttpPost("upload/{userId}")]
    public async Task<IActionResult> Upload(Guid userId)
    {
        if (!CheckUserId(userId))
            return Unauthorized();
        var objectName = GetObjectName(userId);
        var url = await _minio.PresignedPutObjectAsync(
            new PresignedPutObjectArgs()
                .WithBucket(Bucket)
                .WithObject(objectName)
                .WithExpiry(60 * 10) // 10 минут
        );
        return Ok(new { url });
    }

    // ----------------------------
    // Получить ссылку для обновления фото
    // PUT api/Auth/profile-photo/{userId}
    // ----------------------------
    [HttpPut("{userId}")]
    public async Task<IActionResult> Update(Guid userId)
    {
        if (!CheckUserId(userId))
            return Unauthorized();
        var objectName = GetObjectName(userId);
        var url = await _minio.PresignedPutObjectAsync(
            new PresignedPutObjectArgs()
                .WithBucket(Bucket)
                .WithObject(objectName)
                .WithExpiry(60 * 10)
        );
        return Ok(new { url });
    }

    // ----------------------------
    // Удалить фото
    // ----------------------------
    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(Guid userId)
    {
        if (!CheckUserId(userId))
            return Unauthorized();
        var objectName = GetObjectName(userId);
        await _minio.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(Bucket)
                .WithObject(objectName)
        );
        
        return Ok();
    }
}