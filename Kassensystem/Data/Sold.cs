using System.ComponentModel.DataAnnotations.Schema;

namespace Kassensystem.Data;

class Sold 
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set;}
    public Product Item { get; set; }
    public DateTime Zeitpunkt { get; set; }
}