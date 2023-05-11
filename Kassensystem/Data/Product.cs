using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;

namespace Kassensystem.Data;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } 
    public double PriceEuro { get; set; }
    public ProductImage? Image { get; set; }
    public List<Sold>? SellEntries { get; set; }
    public User? User { get; set; }

    //returns true if all non nullable values have been set
    public bool ReadyToSave()
    {
        return Name != null;
    }
    
    
}