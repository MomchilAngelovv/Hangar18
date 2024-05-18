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

	public async Task<Pallet> AddBoxesToPalletAsync(string id, params Box[] boxes)
	{
		var existingPallet = await _db.Pallets.Include(b => b.Boxes).FirstOrDefaultAsync(x => x.Id == id);
		if (existingPallet is null)
		{
			_logger.LogMessage($"Cannot find the pallet with id: {id}");
			return null;
		}

		foreach (var box in boxes)
		{
			existingPallet.Boxes.Add(box);
		}

		await _db.SaveChangesAsync();

		return existingPallet;
	}

	public async Task<List<Pallet>> GetManyAsync()
	{
		return await _db.Pallets.ToListAsync();
	}
}
