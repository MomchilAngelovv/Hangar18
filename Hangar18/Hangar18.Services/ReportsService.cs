using Hangar18.Data;

namespace Hangar18.Services;

public class ReportsService
{
	private readonly PalletsService _palletsService;
	private int nestedLevelCounter = 1;

	public ReportsService(
		PalletsService palletsService)
	{
		_palletsService = palletsService;
	}

	public async Task PrintWarehouseReportAsync()
	{
		var allPallets = await _palletsService.GetManyAsync();

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
