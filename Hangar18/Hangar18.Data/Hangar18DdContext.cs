using Microsoft.EntityFrameworkCore;

namespace Hangar18.Data;

public class Hangar18DdContext : DbContext
{
	public Hangar18DdContext(DbContextOptions options) : base(options)
	{ }

	public Hangar18DdContext()
	{ }

	public DbSet<Pallet> Pallets { get; set; }
	public DbSet<Box> Boxes { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseSqlServer("Server=.;Database=Hangar18_Db;Trusted_Connection=True;TrustServerCertificate=true");
		}

		base.OnConfiguring(optionsBuilder);
	}
}
