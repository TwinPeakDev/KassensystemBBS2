using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kassensystem.Data;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set;}
    public string? Name { get; set; } //TODO:Check for null
    public double PriceEuro { get; set; }
    public ProductImage? Image { get; set; }

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
        if (Image != null)
        {
            var fullPath = Path.Combine(uploadPath,  Image.Name);
            if (!File.Exists(fullPath))
            {
                return "";
            }
        
            return Convert.ToBase64String(File.ReadAllBytes(fullPath));
        }

        return "";
    }
}