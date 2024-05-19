using Hangar18.Data;
using Hangar18.Data.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;

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

	public async Task<Box> CreateBoxAsync(string id)
	{
		var existingBox = await _db.Boxes.AnyAsync(x => x.Id == id);
		if (existingBox)
		{
			_logger.LogMessage($"There is already Box Id: {id}");
			return null;
		}

		var box = new Box(id);

		await _db.Boxes.AddAsync(box);
		await _db.SaveChangesAsync();

		return box;
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
