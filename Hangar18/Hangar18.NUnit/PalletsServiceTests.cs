using Hangar18.Data;
using Hangar18.Services;
using Microsoft.EntityFrameworkCore;

namespace Hangar18.NUnit;

public class PalletsServiceTests
{
	private DbContextOptions<Hangar18DdContext> _options;
	private Hangar18DdContext _db;
	private Logger _logger;

	[SetUp]
	public void Setup()
	{
		_options = new DbContextOptionsBuilder<Hangar18DdContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_logger = new Logger();
	}

	[Test]
	public async Task CreatePalletsAsync_Should_Create_Pallet_For_Every_Given_Id()
	{
		using var _db = new Hangar18DdContext(_options);
		var boxservice = new BoxesService(_db, _logger);
		var _sut = new PalletsService(_db, boxservice, _logger);

		var ids = new List<string> { "TesPallet1", "TestPallet2" };

		await _sut.CreatePalletsAsync(ids);

		var pallets = await _db.Pallets.ToListAsync();

		Assert.That(pallets.Select(b => b.Id).All(ids.Contains), Is.True);
		Assert.That(pallets, Has.Count.EqualTo(2));
	}

	[Test]
	public async Task CreatePalletsAsync_Should_Not_Create_If_Already_Even_Single_One_Exists()
	{
		using var _db = new Hangar18DdContext(_options);
		var boxservice = new BoxesService(_db, _logger);
		var _sut = new PalletsService(_db, boxservice, _logger);

		var ids = new List<string> { "TesPallet1", "TestPallet2" };

		await _sut.CreatePalletsAsync(ids);

		ids = ["TestPallet2", "TestPallet3"];

		var createdPallets = await _sut.CreatePalletsAsync(ids);

		Assert.That(createdPallets, Is.Null);
	}

	[Test]
	public async Task AddBoxesToPalletAsync_Should_Add_Boxes_To_Correct_Pallet()
	{
		using var _db = new Hangar18DdContext(_options);
		var boxservice = new BoxesService(_db, _logger);
		var _sut = new PalletsService(_db, boxservice, _logger);

		var ids = new List<string> { "TesPallet1", "TestPallet2" };
		await _sut.CreatePalletsAsync(ids);

		await _sut.AddBoxesToPalletAsync(ids[0], [new Box("TestBox1"), new Box("TestBox2")]);
		await _sut.AddBoxesToPalletAsync(ids[1], [new Box("TestBox3"), new Box("TestBox4")]);

		var allPallets = await _db.Pallets.Include(p => p.Boxes).ToListAsync();

		Assert.Multiple(() =>
		{
			Assert.That(allPallets, Has.Count.EqualTo(2));
			Assert.That(allPallets.First(p => p.Id == ids[0]).Boxes, Has.Count.EqualTo(2));
			Assert.That(allPallets.First(p => p.Id == ids[1]).Boxes, Has.Count.EqualTo(2));

			Assert.That(allPallets.First(p => p.Id == ids[0]).Boxes.Any(b => b.Id == "TestBox1"), Is.True);
			Assert.That(allPallets.First(p => p.Id == ids[0]).Boxes.Any(b => b.Id == "TestBox2"), Is.True);

			Assert.That(allPallets.First(p => p.Id == ids[1]).Boxes.Any(b => b.Id == "TestBox3"), Is.True);
			Assert.That(allPallets.First(p => p.Id == ids[1]).Boxes.Any(b => b.Id == "TestBox4"), Is.True);
		});
	}

	[Test]
	public async Task AddBoxesToPalletAsync_Should_Not_Add_Not_Unique_Boxes_To_Correct_Pallet()
	{
		using var _db = new Hangar18DdContext(_options);
		var boxservice = new BoxesService(_db, _logger);
		var _sut = new PalletsService(_db, boxservice, _logger);

		var ids = new List<string> { "TesPallet1", "TestPallet2" };
		await _sut.CreatePalletsAsync(ids);

		await _sut.AddBoxesToPalletAsync(ids[0], [new Box("TestBox1"), new Box("TestBox1"), new Box("TestBox1")]);

		var allPallets = await _db.Pallets.Include(p => p.Boxes).ToListAsync();

		Assert.Multiple(() =>
		{
			Assert.That(allPallets, Has.Count.EqualTo(2));
			Assert.That(allPallets.First(p => p.Id == ids[0]).Boxes, Has.Count.EqualTo(1));
			Assert.That(allPallets.First(p => p.Id == ids[1]).Boxes, Has.Count.EqualTo(0));

			Assert.That(allPallets.First(p => p.Id == ids[0]).Boxes.First().Id, Is.EqualTo("TestBox1"));
		});
	}

	[Test]
	public async Task TakeBoxAsync_Should_Only_Take_Itself_When_Empty()
	{
		using var _db = new Hangar18DdContext(_options);
		var boxesService = new BoxesService(_db, _logger);
		var _sut = new PalletsService(_db, boxesService, _logger);

		var palletIds = new List<string> { "TesPallet1", "TestPallet2" };
		var pallets = await _sut.CreatePalletsAsync(palletIds);

		var boxIds = new List<string> { "TestBox1" };
		var boxes = await boxesService.CreateBoxesAsync(boxIds);

		await _sut.AddBoxesToPalletAsync(palletIds[0], [.. boxes]);

		Assert.Multiple(() =>
		{
			Assert.That(boxes, Has.Count.EqualTo(1));
			Assert.That(pallets, Has.Count.EqualTo(2));
			Assert.That(pallets.First(p => p.Id == palletIds[0]).Boxes, Has.Count.EqualTo(1));
		});

		await _sut.TakeBoxAsync("TestBox1");

		boxes = await _db.Boxes.ToListAsync();
		Assert.Multiple(() =>
		{
			Assert.That(boxes, Has.Count.EqualTo(0));
			Assert.That(pallets, Has.Count.EqualTo(2));
			Assert.That(pallets.First(p => p.Id == palletIds[0]).Boxes, Has.Count.EqualTo(0));
		});
	}

	[Test]
	public async Task TakeBoxAsync_Should_Take_Itself_And_All_Nested_When_Have_Nested_Boxes()
	{
		using var _db = new Hangar18DdContext(_options);
		var boxesService = new BoxesService(_db, _logger);
		var _sut = new PalletsService(_db, boxesService, _logger);

		var palletIds = new List<string> { "TesPallet1", "TestPallet2" };
		var pallets = await _sut.CreatePalletsAsync(palletIds);

		var boxIds = new List<string> { "TestBox1", "TestBox2" };
		var boxes = await boxesService.CreateBoxesAsync(boxIds);

		await _sut.AddBoxesToPalletAsync(palletIds[0], [boxes[0]]);
	    await boxesService.AddBoxesToBoxAsync(boxes[0].Id, [boxes[1]]);

		Assert.Multiple(() =>
		{
			Assert.That(boxes, Has.Count.EqualTo(2));
			Assert.That(pallets, Has.Count.EqualTo(2));
			Assert.That(pallets.First(p => p.Id == palletIds[0]).Boxes, Has.Count.EqualTo(1));
			Assert.That(pallets.First(p => p.Id == palletIds[0]).Boxes.First().Boxes, Has.Count.EqualTo(1));
		});

		await _sut.TakeBoxAsync("TestBox1");

		boxes = await _db.Boxes.ToListAsync();
		Assert.Multiple(() =>
		{
			Assert.That(boxes, Has.Count.EqualTo(0));
			Assert.That(pallets, Has.Count.EqualTo(2));
			Assert.That(pallets.First(p => p.Id == palletIds[0]).Boxes, Has.Count.EqualTo(0));
			Assert.That(boxes, Has.Count.EqualTo(0));
		});
	}
}
