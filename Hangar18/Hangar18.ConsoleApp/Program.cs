using Hangar18.Data;
using Hangar18.Services;

Console.WriteLine("Welcome to Hangar 18 - your warehouse from the future!");

var db = new Hangar18DdContext();
var logger = new Logger();
var palletsService = new PalletsService(db, logger);
var boxesService = new BoxesService(db);

await palletsService.CreatePalletAsync("Pallet1");

var allPallets = await palletsService.GetManyAsync();
;




