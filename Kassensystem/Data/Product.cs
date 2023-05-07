using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kassensystem.Data;

public class Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set;}
    public string? Name { get; set; } //TODO:Check for null
    public double PriceEuro { get; set; }
    public string? ImageBinary { get; set; }
    
    //TODO: Pfand
    //TODO: nicht mehr als 5000€
    //TODO: nur bei feature request kosten.
    //TODO: mann kann auch support punkte kaufen
    //TODO: mit marko in der signal gruppe mal drüber render

    public List<Sold>? SellEntries { get; set; }

    //returns true if all non nullable values have been set
    public bool ReadyToSave()
    {
        return Name != null;
    }
    public string GetLocalImageBinary(string webRootPath)
    {
        var imagePath = @"\Uploads";
        var uploadPath = webRootPath + imagePath;
        var imageName = ImageBinary.Substring(0, 10);
        var fullPath = Path.Combine(uploadPath, imageName);
        if (!File.Exists(fullPath))
        {
            File.WriteAllBytes(fullPath, Convert.FromBase64String(ImageBinary));
        }
        
        return Convert.ToBase64String(File.ReadAllBytes(fullPath));
    }
}