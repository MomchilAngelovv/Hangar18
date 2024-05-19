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
		if (_db.Pallets.Any())
		{
			_logger.LogMessage($"Database is not empty");
			return;
		}
		
		//TODO: at the end make it bulk operation
		for (int i = 1; i <= 9; i++)
		{
		    await _boxesService.CreateBoxAsync($"Box{i}");
		}

		for (int i = 1; i <= 3; i++)
		{
			await _palletsService.CreatePalletAsync($"Pallet{i}");
		}

		var allPallets = await _palletsService.GetManyAsync();
		var allBoxes = await _boxesService.GetManyAsync();

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
