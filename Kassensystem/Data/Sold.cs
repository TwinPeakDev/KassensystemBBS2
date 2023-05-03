using System.ComponentModel.DataAnnotations.Schema;

namespace Kassensystem.Data;

public class Sold 
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set;}
    public DateTime Zeitpunkt { get; set; }
    public Product Item { get; set; }
    public User SoldBy { get; set; }
}