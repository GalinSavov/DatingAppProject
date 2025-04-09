using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Interests")]
public class Interest
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<AppUserInterest> UserInterests { get; set; } = [];
}