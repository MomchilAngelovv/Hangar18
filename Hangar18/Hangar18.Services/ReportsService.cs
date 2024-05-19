using Hangar18.Data;
using Microsoft.Extensions.Logging;

namespace Hangar18.Services;

public class ReportsService
{
	private readonly PalletsService _palletsService;
	private readonly Logger _logger;
	private int nestedLevelCounter = 1;

	public ReportsService(
		PalletsService palletsService,
		Logger logger)
	{
		_palletsService = palletsService;
		_logger = logger;
	}

	public async Task PrintWarehouseReportAsync()
	{
		var allPallets = await _palletsService.GetManyAsync();
		_logger.LogMessage("Pallets status report:");
		foreach (var pallet in allPallets)
		{
			await Console.Out.WriteLineAsync($"{pallet.Id}");

			foreach (var box in pallet.Boxes)
			{
				await PrintBoxInfoAsync(box);
				nestedLevelCounter = 1;
			}
		}
	}

	private async Task PrintBoxInfoAsync(Box box)
	{
		await Console.Out.WriteLineAsync($"{new string(' ', nestedLevelCounter * 4)}{box.Id}");

		if (box.Boxes is not null && box.Boxes.Count != 0)
		{
			nestedLevelCounter++;

			foreach (var innerBox in box.Boxes)
			{
				await PrintBoxInfoAsync(innerBox);
			}
		}
	}
}
