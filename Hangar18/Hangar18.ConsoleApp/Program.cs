using Hangar18.Data;
using Hangar18.Services;

Console.WriteLine("Welcome to Hangar 18 - your warehouse from the future!");

var db = new Hangar18DdContext();
var logger = new Logger();
var palletsService = new PalletsService(db, logger);
var boxesService = new BoxesService(db, logger);

var createdPallet = await palletsService.CreatePalletAsync("Pallet P1");
var box1 = await boxesService.CreateBoxAsync("Box 'BC1'", createdPallet.Id);
var box2 = await boxesService.CreateBoxAsync("Box 'BC2'", createdPallet.Id);

await palletsService.AddBoxesToPalletAsync(createdPallet.Id, box1, box2);

var allPallets = await palletsService.GetManyAsync();
;




