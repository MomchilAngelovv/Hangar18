using Hangar18.Data;
using Hangar18.Services;

Console.WriteLine("Welcome to Hangar 18 - your warehouse from the future!");

var db = new Hangar18DdContext();
var logger = new Logger();

var boxesService = new BoxesService(db, logger);
var palletsService = new PalletsService(db, boxesService, logger);
var dataSeeder = new DataSeeder(db, boxesService, palletsService, logger);
var reportsService = new ReportsService(palletsService, logger);

await dataSeeder.SeedDataAsync();

await reportsService.PrintWarehouseReportAsync();

while (true)
{
	var input = await Console.In.ReadLineAsync();
	if (string.IsNullOrWhiteSpace(input))
	{
		logger.LogMessage("Please enter box ids to remove, split by ' ' (space)");
	}

	if (input == "exit")
	{
		await reportsService.PrintWarehouseReportAsync();
		return;
	}

	var boxIds = input?.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

	foreach (var boxId in boxIds)
	{
		await palletsService.TakeBoxAsync(boxId);
	}

	await reportsService.PrintWarehouseReportAsync();
}


