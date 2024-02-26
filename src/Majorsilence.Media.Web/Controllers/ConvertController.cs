using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Media.Web.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class ConvertController : ControllerBase
{
    private readonly ILogger<ConvertController> _logger;
    private readonly Settings _settings;

    public ConvertController(ILogger<ConvertController> logger,
        Settings settings)
    {
        _logger = logger;
        _settings = settings;
    }

    /// <summary>
    ///     Upload multiple files and return their ids.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [RequestSizeLimit(107374182400)] // 100 GB
    [RequestFormLimits(MultipartBodyLengthLimit = 107374182400)] // 100 GB
    public async Task<string> Post(IFormFile file)
    {
        if (file.Length <= 0) return "";

        var fileid = Guid.NewGuid().ToString();
        var ext = Path.GetExtension(file.FileName);
        await using var inputStream = new FileStream(Path.Combine(_settings.UploadFolder,
                $"{fileid}"),
            FileMode.Create);

        var uploadDetailFilePath = Path.Combine(_settings.UploadFolder, $"{fileid}.txt");
        await file.CopyToAsync(inputStream);
        await System.IO.File.WriteAllTextAsync(uploadDetailFilePath, $"{fileid}{Environment.NewLine}{ext}");

        return fileid;
    }
}