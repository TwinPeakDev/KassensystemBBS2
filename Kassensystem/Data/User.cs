using System.ComponentModel.DataAnnotations.Schema;
using Kassensystem.Data;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<Sold> SellEntries { get; set; }
}