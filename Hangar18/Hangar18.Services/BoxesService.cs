using Hangar18.Data;
using Hangar18.Data.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Globalization;

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

	public async Task<List<Box>> CreateBoxesAsync(List<string> ids)
	{
		var existingBox = await _db.Boxes.AnyAsync(b => ids.Contains(b.Id));
		if (existingBox)
		{
			_logger.LogMessage($"There is already existing box with at least one given Id");
			return null;
		}

		var boxes = new List<Box>();
		foreach (var id in ids)
		{
			boxes.Add(new Box(id));
		}

		await _db.Boxes.AddRangeAsync(boxes);
		await _db.SaveChangesAsync();

		return boxes;
	}

	public async Task<Box> AddBoxesToBoxAsync(string id, params Box[] boxes)
	{
		var existingBox = await _db.Boxes.FirstOrDefaultAsync(x => x.Id == id);
		if (existingBox is null)
		{
			_logger.LogMessage($"Cannot find the box with id: {id}");
			return null;
		}

		foreach (var box in boxes)
		{
			box.ParentBoxId = existingBox.Id;
		}

		await _db.SaveChangesAsync();

		return existingBox;
	}

	public async Task<Box> GetOneAsync(string id)
	{
		return await _db.Boxes.Include(b => b.Pallet).FirstOrDefaultAsync(b => b.Id == id);
	}

	public async Task<List<Box>> GetManyAsync()
	{
		return await _db.Boxes.ToListAsync();
	}

	public async Task RemoveBoxesAsync(params Box[] boxes)
	{
		_db.Boxes.RemoveRange(boxes);
		await _db.SaveChangesAsync();
	}
}
