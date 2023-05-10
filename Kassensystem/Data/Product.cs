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
    
    public string GetLocalImageBase64()
    {
        var imageFolder = @"\wwwroot\Uploads";
        var uploadPath = Environment.CurrentDirectory + imageFolder;
        if (ImageName == null) return "";
        
        var fullPath = Path.Combine(uploadPath,  ImageName);

        return !File.Exists(fullPath) ? "" : Convert.ToBase64String(File.ReadAllBytes(fullPath));
    }


    public Tuple<int, int> GetImageWidthAndHeight()
    {
        byte[] imageBytes = Convert.FromBase64String(GetLocalImageBase64());
        
        var ms = new MemoryStream(imageBytes);
        
        ///TODO: system.drawing image from stream
        ///TODO: get height and width
        
        
        return new Tuple<int, int>(0, 0);
    }
}
