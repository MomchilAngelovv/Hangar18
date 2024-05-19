using Hangar18.Data;
using Hangar18.Services;

Console.WriteLine("Welcome to Hangar 18 - your warehouse from the future!");

var db = new Hangar18DdContext();
var logger = new Logger();

var boxesService = new BoxesService(db, logger);
var palletsService = new PalletsService(db, logger);
var dataSeeder = new DataSeeder(db, boxesService, palletsService, logger);
var reportsService = new ReportsService(palletsService);

await dataSeeder.SeedDataAsync();

await reportsService.PrintWarehouseReportAsync();






var allPallet2s = await palletsService.GetManyAsync();
;


