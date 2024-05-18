using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hangar18.Data;

public class Box
{
    public Box(string id, string palletId)
    {
        Id = id;
        PalletId = palletId;

	}

    [Key]
    public string Id { get; init; }

    public string? ParentBoxId { get; set; }
    public virtual Box ParentBox { get; set; }
    public string PalletId { get; set; }
    public virtual Pallet Pallet { get; set; }
}
