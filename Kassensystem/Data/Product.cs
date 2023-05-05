using System.ComponentModel.DataAnnotations.Schema;

namespace Kassensystem.Data;

public class Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set;}
    public string? Name { get; set; } //TODO:Check for null
    public double PriceEuro { get; set; }
    public string? Image { get; set; }
    public List<Sold>? SellEntries { get; set; }

    //returns true if all non nullable values have been set
    public bool ReadyToSave()
    {
        return Name != null;
    }
}