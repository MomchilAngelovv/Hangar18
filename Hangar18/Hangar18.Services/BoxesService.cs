using Hangar18.Data;

namespace Hangar18.Services;

public class BoxesService
{
	private readonly Hangar18DdContext _db;

	public BoxesService(Hangar18DdContext db)
	{
		_db = db;
	}
}
