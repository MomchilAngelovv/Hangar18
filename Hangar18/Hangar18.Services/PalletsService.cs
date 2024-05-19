﻿using Microsoft.EntityFrameworkCore;
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

	public async Task TakeBoxAsync(string boxId)
	{
		var box = await _boxesService.GetOneAsync(boxId);

		if (box is null)
		{
			_logger.LogMessage($"Cannot find box with id: {boxId} to remove it.");
			return; 
		}

		var openedBoxes = new List<Box>();
		var boxesToPutBackOnPallet = new List<Box>();
		var palletId = string.Empty;

		if (box.Boxes is not null && box.Boxes.Count > 0)
		{
			openedBoxes.AddRange(box.Boxes);
		}

		while (box.Pallet is null)
		{
			if (box.ParentBoxId is not null)
			{
				openedBoxes.Add(box.ParentBox);
				boxesToPutBackOnPallet.AddRange(box.ParentBox.Boxes.Where(b => b.Id != boxId).Except(openedBoxes));
			}

			box = await _boxesService.GetOneAsync(box.ParentBox.Id);
		}

		palletId = box.Pallet.Id;
		openedBoxes.Add(box);

		await AddBoxesToPalletAsync(palletId, [.. boxesToPutBackOnPallet]);
		await _boxesService.DestroyOpenedBoxesAsync([.. openedBoxes]);
	}

	public async Task<List<Pallet>> GetManyAsync()
	{
		return await _db.Pallets.Include(p => p.Boxes).ThenInclude(b => b.Boxes).ToListAsync();
	}

	
}
