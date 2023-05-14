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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Net.Mime;

namespace Kassensystem.Data;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set;}
    public string Name { get; set; } 
    public double PriceEuro { get; set; }
    public string? ImageName { get; set; }
    public List<Sold>? SellEntries { get; set; }
    public User? User { get; set; }

    //returns true if all non nullable values have been set
    public bool ReadyToSave()
    {
        return Name != null;
    }
    
    public string GetLocalImageBase64(IWebHostEnvironment environment)
    {
        var imagePath = @"Uploads";
        string uploadPath;
        //if(true)
        if(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != null && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")!.Equals("true"))
            uploadPath = Path.Combine(@"wwwroot", imagePath);
        else
            uploadPath = Path.Combine(environment.WebRootPath, imagePath);

        if (ImageName == null) return "";
        
        var fullPath = Path.Combine(uploadPath,  ImageName);

        return !File.Exists(fullPath) ? "" : Convert.ToBase64String(File.ReadAllBytes(fullPath));
    }


    public Tuple<int, int> GetImageWidthAndHeight(IWebHostEnvironment environment)
    {
        byte[] imageBytes = Convert.FromBase64String(GetLocalImageBase64(environment));
        
        var ms = new MemoryStream(imageBytes);
        
        ///TODO: system.drawing image from stream
        ///TODO: get height and width
        
        
        return new Tuple<int, int>(0, 0);
    }
}