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
        return $"{userId}.jpg";
    }

    // ----------------------------
    // Добавить фото
    // POST api/profile-photo/upload/{userId}
    // ----------------------------
    [HttpPost("upload/{userId}")]
    public async Task<IActionResult> Upload(Guid userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Empty file");
        CheckUserId(userId);
        var objectName = GetObjectName(userId);

        using var stream = file.OpenReadStream();

        await _minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(Bucket)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType));

        return Ok();
    }

    // ----------------------------
    // Получить фото
    // GET api/profile-photo/{userId}
    // ----------------------------
    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(Guid userId)
    {
        var objectName = GetObjectName(userId);

        MemoryStream ms = new();

        await _minio.GetObjectAsync(new GetObjectArgs()
            .WithBucket(Bucket)
            .WithObject(objectName)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(ms);
            }));

        ms.Position = 0;

        return File(ms, "image/jpeg");
    }

    // ----------------------------
    // Удалить фото
    // DELETE api/profile-photo/{userId}
    // ----------------------------
    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(Guid userId)
    {
        CheckUserId(userId);
        var objectName = GetObjectName(userId);

        await _minio.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(Bucket)
            .WithObject(objectName));

        return Ok();
    }

    // ----------------------------
    // Изменить фото
    // PUT api/profile-photo/{userId}
    // ----------------------------
    [HttpPut("{userId}")]
    public async Task<IActionResult> Update(Guid userId, IFormFile file)
    {
        CheckUserId(userId);
        await Delete(userId);
        return await Upload(userId, file);
    }

    public bool CheckUserId(Guid userId)
    {
        Guid companyId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return  companyId.Equals(userId);
    }
}