namespace Hangar18.Services;

public class ReportsService
{
	private readonly PalletsService _palletsService;

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
                await Console.Out.WriteLineAsync($"    {box.Id}");
            }
        }
	}
}
