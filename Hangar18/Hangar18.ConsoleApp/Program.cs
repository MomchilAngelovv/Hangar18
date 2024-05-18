﻿using Hangar18.Data;
using Hangar18.Services;

Console.WriteLine("Welcome to Hangar 18 - your warehouse from the future!");

var db = new Hangar18DdContext();
var logger = new Logger();

var boxesService = new BoxesService(db, logger);
var palletsService = new PalletsService(db, logger);
var dataSeeder = new DataSeeder(db, boxesService, palletsService, logger);
var reportsService = new ReportsService(palletsService);

await dataSeeder.SeedDataAsync();

var allPallets = await palletsService.GetManyAsync();
var allBoxes = await boxesService.GetManyAsync();

await palletsService.AddBoxesToPalletAsync(allPallets[0].Id, allBoxes[0], allBoxes[3]);
await boxesService.AddBoxesToBoxAsync(allBoxes[0].Id, allBoxes[1], allBoxes[2]);
await boxesService.AddBoxesToBoxAsync(allBoxes[3].Id, allBoxes[4], allBoxes[5], allBoxes[6]);

await reportsService.PrintWarehouseReportAsync();

var allPallet2s = await palletsService.GetManyAsync();
;


