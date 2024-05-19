using Hangar18.Data;

namespace Hangar18.Services;

public class DataSeeder
{
	private readonly Hangar18DdContext _db;
	private readonly BoxesService _boxesService;
	private readonly PalletsService _palletsService;
	private readonly Logger _logger;

	public DataSeeder(
		Hangar18DdContext db,
		BoxesService boxesService,
		PalletsService palletsService,
		Logger logger)
	{
		_db = db;
		_boxesService = boxesService;
		_palletsService = palletsService;
		_logger = logger;
	}

	public async Task SeedDataAsync()
	{
		var boxIds = new List<string>();
		for (int i = 1; i <= 9; i++)
		{
			boxIds.Add($"Box{i}");
		}

		await _boxesService.CreateBoxesAsync(boxIds);

		var palletIds = new List<string>();
		for (int i = 1; i <= 3; i++)
		{
			palletIds.Add($"Pallet{i}");
		}

		await _palletsService.CreatePalletsAsync(palletIds);

		var allPallets = await _palletsService.GetManyAsync();
		var allBoxes = await _boxesService.GetManyAsync();

		if (allBoxes.Count == 0 || allBoxes.Count == 0)
		{
			_logger.LogMessage("Database is not empty. Aborting seeding");
			return;
		}

		//Pallet 1
		await _palletsService.AddBoxesToPalletAsync(allPallets[0].Id, allBoxes[0], allBoxes[1]);
		await _boxesService.AddBoxesToBoxAsync(allBoxes[0].Id, allBoxes[2], allBoxes[3], allBoxes[4]);
		await _boxesService.AddBoxesToBoxAsync(allBoxes[1].Id, allBoxes[5]);
		await _boxesService.AddBoxesToBoxAsync(allBoxes[5].Id, allBoxes[6], allBoxes[7]);

		//Palet 2
		await _palletsService.AddBoxesToPalletAsync(allPallets[1].Id, allBoxes[8]);

		//Palet 3 - empty
		_logger.LogMessage($"Database seeded successfully");
	}
}
