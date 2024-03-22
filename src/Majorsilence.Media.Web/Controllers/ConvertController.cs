using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using UUIDNext;

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
    /// Initiate a new file id.  This most be called before uploading a file.  The returned value
    /// must be sent in the Post method as a header with the name "FileId".
    /// </summary>
    /// <returns></returns>
    public string Get()
    {
        // The purpose of this method is to permit the client to get a file id
        // to track the file upload.  This is to prevent a client from uploading
        // a file and then not sending the file to be processed.  This method
        // will create a file in the upload folder with the file id as the name
        // and the contents will be the token.  The client must send the token
        // in the header of the Post method.  The token is the bearer token
        // from the client.
        // The WorkerService will then process the file and delete the file
        // after processing.  If the file is not processed then the file will
        // remain in the upload folder and the client can try again.
        var id = Uuid.NewDatabaseFriendly(Database.Other).ToString();
        var startRequestPath = Path.Combine(_settings.UploadFolder, $"{id}.startrequest");
        System.IO.File.WriteAllText(startRequestPath, GetBearerToken());
        return id;
    }

    /// <summary>
    ///  Upload a file and return its id.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [RequestSizeLimit(107374182400)] // 100 GB
    [RequestFormLimits(MultipartBodyLengthLimit = 107374182400)] // 100 GB
    public async Task<IActionResult> Post(IFormFile file)
    {
        if (file.Length <= 0) return BadRequest("File is empty");

        var fileid = Request.Headers["FileId"].ToString();
        if (string.IsNullOrWhiteSpace(fileid))
        {
            return BadRequest("The file id must be sent in the header with the name 'FileId'");
        }

        if (fileid.Trim().Length != 36)
        {
            return BadRequest("Invalid file id.  Must be retrieved by calling GET.");
        }

        var token = GetBearerToken();
        var startRequestPath = Path.Combine(_settings.UploadFolder, $"{fileid}.startrequest");
        if (!System.IO.File.Exists(startRequestPath))
        {
            return BadRequest("Invalid file id.  Must be retrieved by calling Get.");
        }
        string startRequestToken = System.IO.File.ReadAllText(startRequestPath);
        if (!string.Equals(startRequestToken, token))
        {
            return BadRequest("Invalid token.  Tokens must match between GET and POST requests.");
        }
        
        var ext = Path.GetExtension(file.FileName);
        await using var inputStream = new FileStream(Path.Combine(_settings.UploadFolder,
                $"{fileid}"),
            FileMode.Create);

        var uploadDetailFilePath = Path.Combine(_settings.UploadFolder, $"{fileid}.txt");
        await file.CopyToAsync(inputStream);
        await System.IO.File.WriteAllTextAsync(uploadDetailFilePath, $"{fileid}{Environment.NewLine}{ext}");

        return Ok(fileid);
    }

    private string GetBearerToken()
    {
        string authorizationHeader = Request.Headers["Authorization"];
        string[] parts = authorizationHeader.Split(' ');
        string scheme = parts[0];
        string token = parts[1];
        return token;
    }
}