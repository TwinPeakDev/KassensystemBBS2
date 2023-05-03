using System.ComponentModel.DataAnnotations.Schema;

namespace Kassensystem.Data;

class Product
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set;}
    public string Name { get; set; }
    public double PriceEur { get; set; }
    public string Image { get; set; }
    public List<Sold> SellEntries { get; set; }
}