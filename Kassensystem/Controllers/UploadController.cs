using Microsoft.AspNetCore.Mvc;

namespace Kassensystem.Controllers;

[DisableRequestSizeLimit]
public class UploadController : Controller
{

    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadController> _logger;

    public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpPost("upload/single")]
    public IActionResult Single(IFormFile file)
    {
        try
        {
            UploadFile(file);
            return StatusCode(200);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    public async Task UploadFile(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var imagePath = @"\Uploads";
            var uploadPath = _environment.WebRootPath + imagePath;
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fullPath = Path.Combine(uploadPath, file.FileName.Replace(" ", ""));
            await using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
                fileStream.Flush();
                await fileStream.DisposeAsync();
                fileStream.SafeFileHandle.Dispose();
            }
        }
    }
}