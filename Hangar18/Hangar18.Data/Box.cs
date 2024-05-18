using System.ComponentModel.DataAnnotations;

namespace Hangar18.Data;

public class Box
{
    public Box(string id)
    {
        Id = id;
	}

    [Key]
    public string Id { get; init; }

    public string? ParentBoxId { get; set; }
    public virtual Box ParentBox { get; set; }
    public string? PalletId { get; set; }
    public virtual Pallet Pallet { get; set; }

    public virtual ICollection<Box> Boxes { get; set; }
}
