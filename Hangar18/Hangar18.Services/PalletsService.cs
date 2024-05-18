using Microsoft.EntityFrameworkCore;
using Hangar18.Data;

namespace Hangar18.Services;

public class PalletsService
{
	private readonly Hangar18DdContext _db;
	private readonly Logger _logger;

	public PalletsService(
		Hangar18DdContext db,
		Logger logger)
	{
		_db = db;
		_logger = logger;
	}

	public async Task<Pallet> CreatePalletAsync(string id)
	{
		var existingPallet = await _db.Pallets.AnyAsync(x => x.Id == id);
		if (existingPallet)
		{
			_logger.LogMessage("There is already Pallet with this Id");
			return null;
		}

		var pallet = new Pallet(id);

		await _db.Pallets.AddAsync(pallet);
		await _db.SaveChangesAsync();

		return pallet;
	}

	public async Task<List<Pallet>> GetManyAsync()
	{
		return await _db.Pallets.ToListAsync();
	}
}
