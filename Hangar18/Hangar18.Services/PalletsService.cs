using Microsoft.EntityFrameworkCore;
using Hangar18.Data;
using System.Runtime.InteropServices;

namespace Hangar18.Services;

public class PalletsService
{
	private readonly Hangar18DdContext _db;
	private readonly BoxesService _boxesService;
	private readonly Logger _logger;

	public PalletsService(
		Hangar18DdContext db,
		BoxesService boxesService,
		Logger logger)
	{
		_db = db;
		_boxesService = boxesService;
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

	public async Task OpenBoxAsync(string boxId)
	{
		var box = await _boxesService.GetOneAsync(boxId);

		if (box is null)
		{
			_logger.LogMessage($"Cannot find box with id: {boxId} to remove it.");
			return; 
		}

		box.Pallet = null;
		await _db.SaveChangesAsync();
	}

	public async Task<List<Pallet>> GetManyAsync()
	{
		return await _db.Pallets.Include(p => p.Boxes).ThenInclude(b => b.Boxes).ToListAsync();
	}
}
