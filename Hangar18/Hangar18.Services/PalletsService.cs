using Microsoft.EntityFrameworkCore;
using Hangar18.Data;
using System.Runtime.InteropServices;
using System.Globalization;
using Microsoft.IdentityModel.Abstractions;

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

	public async Task<List<Pallet>> CreatePalletsAsync(List<string> ids)
	{
		var existingPallet = await _db.Pallets.AnyAsync(b => ids.Contains(b.Id));
		if (existingPallet)
		{
			_logger.LogMessage($"There is already existing pallet with at least one given Id");
			return null;
		}

		var pallets = new List<Pallet>();
		foreach (var id in ids)
		{
			pallets.Add(new Pallet(id));
		}

		await _db.Pallets.AddRangeAsync(pallets);
		await _db.SaveChangesAsync();

		return pallets;
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

	public async Task TakeBoxAsync(string boxId)
	{
		var box = await _boxesService.GetOneAsync(boxId);

		if (box is null)
		{
			_logger.LogMessage($"Cannot find box with id: {boxId} to remove it.");
			return; 
		}

		_logger.LogMessage($"Removing box with Id: {boxId} and all of its nested boxes (if any) from pallet");

		var removedBoxes = new List<Box>
		{
			box
		};

		var newPalletBoxes = new List<Box>();

		var palletId = string.Empty;

		if (box.Boxes is not null && box.Boxes.Count > 0)
		{
			foreach (var boxToRemove in box.Boxes)
			{
				removedBoxes.AddRange(await AddBoxForRemovalAsync(boxToRemove));
			}

			removedBoxes.AddRange(box.Boxes);
		}

		while (box.Pallet is null)
		{
			if (box.ParentBoxId is not null)
			{
				removedBoxes.Add(box.ParentBox);
				newPalletBoxes.AddRange(box.ParentBox.Boxes.Where(b => b.Id != boxId).Except(removedBoxes));
			}

			box = await _boxesService.GetOneAsync(box.ParentBox.Id);
		}

		palletId = box.Pallet.Id;
		removedBoxes.Add(box);

		await AddBoxesToPalletAsync(palletId, [.. newPalletBoxes]);
		await _boxesService.RemoveBoxesAsync([.. removedBoxes]);
	}

	private async Task<List<Box>> AddBoxForRemovalAsync(Box box)
	{
		var boxes = new List<Box>();

		while (box.Boxes is not null && box.Boxes.Count > 0)
		{
			boxes.AddRange(box.Boxes);

			foreach (var boxToRemove in box.Boxes)
			{
				await AddBoxForRemovalAsync(box);
			}
		}

		return boxes;
	}

	public async Task<List<Pallet>> GetManyAsync()
	{
		return await _db.Pallets.Include(p => p.Boxes).ThenInclude(b => b.Boxes).ToListAsync();
	}
}
