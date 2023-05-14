using Microsoft.AspNetCore.Mvc;

namespace Kassensystem.Controllers;

[DisableRequestSizeLimit]
[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{

    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadController> _logger;

    public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpPost("single")]
    public async Task<IActionResult> Single(IFormFile file)
    {
        return await UploadFile(file);
    }
    
    public async Task<ObjectResult> UploadFile(IFormFile? file)
    {
        if (file != null && file.Length > 0)
        {
            try
            {
                var imagePath = @"Uploads";
                string uploadPath;
                //if(true)
                if(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != null && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")!.Equals("true"))
                    uploadPath = Path.Combine(@"wwwroot", imagePath);
                else
                    uploadPath = Path.Combine(_environment.WebRootPath, imagePath);
                
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var fullPath = Path.Combine(uploadPath, file.FileName.Replace(" ", ""));
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Close();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        else
        {
            return StatusCode(416, "File not There");
        }

        return StatusCode(400, "");
    }
}