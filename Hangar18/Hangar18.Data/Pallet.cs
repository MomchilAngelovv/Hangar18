using System.ComponentModel.DataAnnotations;

namespace Hangar18.Data;

public class Pallet
{
    public Pallet(string id)
    {
        Id = id;
    }

    [Key]
    public string Id { get;  init; }

    public virtual ICollection<Box> Boxes { get; } 
}
