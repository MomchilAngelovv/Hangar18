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

    public string PalletId { get; set; }
    public virtual Pallet Pallet { get; set; }
}
