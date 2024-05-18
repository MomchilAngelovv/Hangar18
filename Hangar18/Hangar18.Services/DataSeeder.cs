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

	/// <summary>
	/// This will seed 2 pallets and 10 boxes 
	/// </summary>
	/// <returns></returns>
	public async Task SeedDataAsync()
	{
		if (_db.Pallets.Any())
		{
			_logger.LogMessage($"Database is not empty");
		}

		for (int i = 1; i <= 10; i++)
		{
		    await _boxesService.CreateBoxAsync($"Box 'BC{i}'");
		}

		for (int i = 1; i <= 2; i++)
		{
			await _palletsService.CreatePalletAsync($"Pallet P{i}");
		}

		_logger.LogMessage($"Database seeded successfully");
	}
}
