using Microsoft.AspNetCore.Mvc;

namespace Majorsilence.Media.Web.Controllers
{

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
        /// Upload multiple files and return their ids.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(107374182400)] // 100 GB
        [RequestFormLimits(MultipartBodyLengthLimit = 107374182400)] // 100 GB
        public async Task<string> Post(IFormFile file)
        {
           
            if (file.Length > 0)
            {
                string fileid = Guid.NewGuid().ToString();
                string ext = Path.GetExtension(file.FileName);
                using var inputStream = new FileStream(Path.Combine(_settings.UploadFolder,
                    $"{fileid}{ext}"),
                    FileMode.Create);

                await file.CopyToAsync(inputStream);
                return fileid;
            }

            return "";
        }
    }
}