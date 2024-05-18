using Hangar18.Data;

namespace Hangar18.Services;

public class PalletsService
{
	private readonly Hangar18DdContext _db;

	public PalletsService(Hangar18DdContext db)
	{
		_db = db;
	}

	public void CreatePallet(string id)
	{
		var pallet = new Pallet(id);
	}
}
