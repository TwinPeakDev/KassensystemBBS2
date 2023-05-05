using System.ComponentModel.DataAnnotations.Schema;

namespace Kassensystem.Data;

public class Sold 
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set;}
    public DateTime Created { get; set; } = DateTime.Now;
    public Product? Item { get; set; } //TODO:Check for null
    public User? SoldBy { get; set; }//TODO:Check for null
}