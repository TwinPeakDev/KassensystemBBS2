using System.ComponentModel.DataAnnotations.Schema;
using Kassensystem.Data;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? FirstName { get; set; } //TODO:Check for null
    public string? LastName { get; set; } //TODO:Check for null
    public List<Sold>? SellEntries { get; set; } //TODO:Check for null
}