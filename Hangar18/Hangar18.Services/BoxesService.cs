using Hangar18.Data;
using Microsoft.EntityFrameworkCore;

namespace Hangar18.Services;

public class BoxesService
{
	private readonly Hangar18DdContext _db;
	private readonly Logger _logger;

	public BoxesService(
		Hangar18DdContext db,
		Logger logger)
	{
		_db = db;
		_logger = logger;
	}

	public async Task<Box> CreateBoxAsync(string id, string palletId)
	{
		var existingBox = await _db.Boxes.AnyAsync(x => x.Id == id);
		if (existingBox)
		{
			_logger.LogMessage($"There is already Box Id: {id}");
			return null;
		}

		var existingPallet = await _db.Pallets.AnyAsync(x => x.Id == palletId);
		if (!existingPallet)
		{
			_logger.LogMessage($"Cannot find pallet with Id: {palletId}");
			return null;
		}

		var box = new Box(id, palletId);

		await _db.Boxes.AddAsync(box);
		await _db.SaveChangesAsync();

		return box;
	}
}
