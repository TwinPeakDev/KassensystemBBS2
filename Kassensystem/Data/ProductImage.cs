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
using System.Net;
using RestSharp;
using RestSharp.Authenticators;

namespace Kassensystem.Data;

public class ProductImage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public ProductImage(string imageName)
    {
        ImageName = imageName;
    }

    private string _imageName;

    public string ImageName
    {
        get => _imageName;
        set
        {
            _imageName = value;
            InitializeImage();
        }
    }

    public (int, int) WidthAndHeight { get; private set; }
    public (int, int) AspectRatio { get; private set; }

    private void InitializeImage()
    {
        CalculateWidthAndHeight();
        CalculateRatio();
    }
    public string GetLocalImageBase64()
    {
        
        var imagePath = @"Uploads";
        string uploadPath;
        //if(true)
        if(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != null && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")!.Equals("true"))
            uploadPath = Path.Combine(@"wwwroot", imagePath);
        else 
            uploadPath = Path.Combine(Environment.GetEnvironmentVariable("WWWROOT")!, imagePath);

        if (ImageName == null) return "";
        
        var fullPath = Path.Combine(uploadPath,  ImageName);

        return !File.Exists(fullPath) ? "" : Convert.ToBase64String(File.ReadAllBytes(fullPath));
    }

    private async void CalculateWidthAndHeight()
    {
        var imageBase64 = GetLocalImageBase64();

        if (imageBase64 == "")
        {
            WidthAndHeight = (0, 0);
            return;
        }
        
        var image = Image.Load(new MemoryStream(Convert.FromBase64String(imageBase64)));

        WidthAndHeight = (image.Width, image.Height);
    }

    private void CalculateRatio()
    {
        var startTime = DateTime.Now;
        if (WidthAndHeight.Item1 == 0 || WidthAndHeight.Item2 == 0)
        {
            AspectRatio = (0, 0);
            return;
        }

        int larger, smaller;
        
        if (WidthAndHeight.Item1 > WidthAndHeight.Item2)
        {
            larger = WidthAndHeight.Item1;
            smaller = WidthAndHeight.Item2;
        }
        else
        {
            larger = WidthAndHeight.Item2;
            smaller = WidthAndHeight.Item1;
        }
        
        //get greatest common divisor using euclidean algorithm
        
        double remainderLarge = larger % smaller;
        double remainderSmall = smaller % remainderLarge;

        double last = 0;
        
        while (remainderSmall != 0)
        {
            last = remainderSmall;
            remainderSmall = remainderLarge % remainderSmall;
            remainderLarge = remainderSmall;
        }

        
        Console.WriteLine((DateTime.Now - startTime).Milliseconds);
        AspectRatio = (WidthAndHeight.Item1 / (int)last, WidthAndHeight.Item2 / (int)last);
    }
    

}