
/*
Copyright (C) 2023
Elias Stepanik: https://github.com/eliasstepanik
Olivia Streun: https://github.com/nnuuvv

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see https://www.gnu.org/licenses/.
*/

﻿using Microsoft.AspNetCore.Mvc;

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
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
                fileStream.Close();
            }
        }
    }
}