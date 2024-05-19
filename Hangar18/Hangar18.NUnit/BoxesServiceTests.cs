using Hangar18.Data;
using Hangar18.Services;
using Microsoft.EntityFrameworkCore;

namespace Hangar18.NUnit;

public class BoxesServiceTests
{
	private DbContextOptions<Hangar18DdContext> _options;
	private Hangar18DdContext _db;
	private BoxesService _sut;
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
	public async Task CreateBoxesAsync_Should_Create_Box_For_Every_Given_Id()
	{
		using var _db = new Hangar18DdContext(_options);
		_sut = new BoxesService(_db, _logger);

		var ids = new List<string> { "TestBox1", "TestBox2" };

		await _sut.CreateBoxesAsync(ids);

		var boxes = await _db.Boxes.ToListAsync();

		Assert.Multiple(() =>
		{
			Assert.That(boxes.Select(b => b.Id).All(ids.Contains), Is.True);
			Assert.That(boxes, Has.Count.EqualTo(2));
		});
	}

	[Test]
	public async Task CreateBoxesAsync_Should_Not_Create_If_Already_Even_Single_One_Exists()
	{
		using var _db = new Hangar18DdContext(_options);
		_sut = new BoxesService(_db, _logger);

		var ids = new List<string> { "TestBox1", "TestBox2" };

		await _sut.CreateBoxesAsync(ids);
		
		ids = ["TestBox2", "TestBox3"];

		var creaedBoxes = await _sut.CreateBoxesAsync(ids);

		Assert.That(creaedBoxes, Is.Null);
	}

	[Test]
	public async Task AddBoxesToBoxAsync_Should_Add_Nested_Boxes()
	{
		using var _db = new Hangar18DdContext(_options);
		_sut = new BoxesService(_db, _logger);

		var ids = new List<string> { "TestBox1", "TestBox2" };
		var boxes = await _sut.CreateBoxesAsync(ids);
		await _sut.AddBoxesToBoxAsync(boxes[0].Id, [boxes[1]]);

		Assert.Multiple(() =>
		{
			Assert.That(boxes, Has.Count.EqualTo(2));
			Assert.That(boxes.First(p => p.Id == ids[0]).Boxes, Has.Count.EqualTo(1));
			Assert.That(boxes.First(p => p.Id == ids[1]).ParentBoxId, Is.EqualTo("TestBox1"));
		});
	}
}
